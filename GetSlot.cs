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
                string slotIDString = req.Query["SlotID"].ToString();
                if (slotIDString != null && !slotIDString.Equals("")) {
                    Guid SlotID = Guid.Parse(slotIDString);            
                    Slot slot = await new SlotService().FindSlot(log, tblSlot, SlotID);
                    if (slot == null) {
                        return new NotFoundResult();
                    }
                    responseMessage = JsonConvert.SerializeObject(slot);
                } else {
                    Slot[] slots = await new SlotService().FindAll(log, tblSlot);
                    responseMessage = JsonConvert.SerializeObject(slots);                
                }
            }

            return new OkObjectResult(responseMessage);
        }
    }
}
