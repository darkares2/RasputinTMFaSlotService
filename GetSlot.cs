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

            string responseMessage = null;
            string UserIDString = req.Query["UserID"].ToString();
            if (UserIDString != null && !UserIDString.Equals("")) {
                Slot[] slots = await new SlotService().FindUserSlots(log, tblSlot, Guid.Parse(UserIDString));
                responseMessage = JsonConvert.SerializeObject(slots);                
            } else {
                Guid SlotID = Guid.Parse(req.Query["SlotID"].ToString());            
                Slot slot = await new SlotService().FindSlot(log, tblSlot, SlotID);
                if (slot == null) {
                    return new NotFoundResult();
                }
                responseMessage = JsonConvert.SerializeObject(slot);
            }

            return new OkObjectResult(responseMessage);
        }
    }
}
