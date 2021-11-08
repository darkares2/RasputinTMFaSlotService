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
    public static class CreateSlot
    {
        [FunctionName("CreateSlot")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)] HttpRequest req,
                                                    [Table("tblSlots")] CloudTable tblSlot,
                                                    ILogger log)
        {
            log.LogInformation("CreateSlot called");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            DateTime timeslot = data?.timeslot;
            Guid userID = data?.userID;
            Guid[] serviceIDs = data?.serviceIDs;

            Slot Slot = await new SlotService().InsertSlot(log, tblSlot, timeslot, userID, serviceIDs);

            string responseMessage = JsonConvert.SerializeObject(Slot);
            return new OkObjectResult(responseMessage);
        }
    }
}
