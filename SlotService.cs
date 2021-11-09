using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Logging;

namespace Rasputin.TM {
    public class SlotService {
        public async Task<Slot> InsertSlot(ILogger log, CloudTable tblSlot, DateTime timeslot, Guid userID, Guid[] serviceIDs)
        {
            Slot Slot = new Slot(timeslot, userID.ToString(), serviceIDs);
            TableOperation operation = TableOperation.Insert(Slot);
            await tblSlot.ExecuteAsync(operation);
            return Slot;
        }

        public async Task<Slot> FindSlot(ILogger log, CloudTable tblSlot, Guid slotID)
        {
            string pk = "p1";
            string rk = slotID.ToString();
            log.LogInformation($"FindSlot: {pk},{rk}");
            TableOperation operation = TableOperation.Retrieve(pk, rk);
            try {
                return (Slot)await tblSlot.ExecuteAsync(operation);
            } catch(Exception ex) {
                log.LogWarning(ex, "FindSlot", slotID);
                return null;
            }
        }

        public async Task<Slot[]> FindUserSlots(ILogger log, CloudTable tblSlot, Guid userID)
        {
            log.LogInformation($"All");
            List<Slot> result = new List<Slot>();
            TableQuery<Slot> query = new TableQuery<Slot>().Where(TableQuery.GenerateFilterCondition("UserID", QueryComparisons.Equal, userID.ToString()));
            TableContinuationToken continuationToken = null;
            try {
                do {
                var page = await tblSlot.ExecuteQuerySegmentedAsync(query, continuationToken);
                continuationToken = page.ContinuationToken;
                result.AddRange(page.Results);
                } while(continuationToken != null);
                return result.ToArray();
            } catch(Exception ex) {
                log.LogWarning(ex, "All");
                return null;
            }
        }

        public async Task DeleteSlot(ILogger log, CloudTable tblSlot, Slot slot) 
        {
            log.LogInformation($"DeleteSlot: {slot.SlotID}");
            TableOperation operation = TableOperation.Delete(slot);
            await tblSlot.ExecuteAsync(operation);
        }
    }
}