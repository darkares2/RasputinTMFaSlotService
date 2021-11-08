using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Cosmos.Table;

namespace Rasputin.TM
{
    public static class GetSlot
    {
        [FunctionName("GetSlot")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)] HttpRequest req,
                                                    [Table("tblSlots")] CloudTable tblSlot,
                                                    ILogger log)
        {
            log.LogInformation("GetSlot called");

            Guid SlotID = Guid.Parse(req.Query["SlotID"].ToString());            
            Slot Slot = await new SlotService().FindSlot(log, tblSlot, SlotID);
            if (Slot == null) {
                return new NotFoundResult();
            }
            string responseMessage = JsonConvert.SerializeObject(Slot);

            return new OkObjectResult(responseMessage);
        }
    }
}
