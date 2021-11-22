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
using System.Linq;

namespace Rasputin.TM
{
    public static class CreateSlot
    {
        [FunctionName("CreateSlot")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
                                                    [Table("tblSlots")] CloudTable tblSlot,
                                                    ILogger log)
        {
            log.LogInformation("CreateSlot called");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            CreateSlotRequest data = (CreateSlotRequest)JsonConvert.DeserializeObject(requestBody, typeof(CreateSlotRequest));
            
            Slot Slot = await new SlotService().InsertSlot(log, tblSlot, data.Timeslot, data.UserID, data.ServiceIDs);

            string responseMessage = JsonConvert.SerializeObject(Slot);
            return new OkObjectResult(responseMessage);
        }
    }
}
