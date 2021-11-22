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
    public static class HandleAppointmentFromSlot
    {
        [FunctionName("HandleAppointmentFromSlot")]
        public static async Task Run([QueueTrigger("appointmentFromSlotqueue")] string appointmentFromSlotqueueItem,
                                     [Queue("appointmentCreateQueue")] IAsyncCollector<string> appointmentCreateQueue,
                                     [Queue("userMessageQueue")] IAsyncCollector<string> userMessageQueue,
                                     [Table("tblSlots")] CloudTable tblSlot,
                                     ILogger log)
        {
            log.LogInformation("HandleAppointmentFromSlot called");

            dynamic data = JsonConvert.DeserializeObject(appointmentFromSlotqueueItem);
            Guid slotID = data?.SlotID;
            Guid userID = data?.UserID;

            Slot slot = await new SlotService().FindSlot(log, tblSlot, slotID);
            if (slot != null) {
                data.Timeslot = slot.Timeslot;
                data.SlotUserID = slot.UserID;
                await new SlotService().DeleteSlot(log, tblSlot, slot);
                await appointmentCreateQueue.AddAsync(JsonConvert.SerializeObject(data));
            } else {
                await userMessageQueue.AddAsync(JsonConvert.SerializeObject(new UserMessage(userID, "Slot cannot be taken, please try another")));
            }
        }
    }
}
