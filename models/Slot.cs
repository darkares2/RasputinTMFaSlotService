
using System;
using System.Linq;
using Microsoft.Azure.Cosmos.Table;

namespace Rasputin.TM{
    public class Slot : TableEntity {
        public Slot(DateTime timeslot, string userID, Guid[] serviceIDs)
        {
            this.PartitionKey = "p1";
            this.RowKey = Guid.NewGuid().ToString();
            this.UserID = userID;
            this.ServiceIDs = serviceIDs == null ? "" : string.Join(",", serviceIDs.Select(x => x.ToString())); 
        }
        Slot() { }
        public DateTime? Timeslot { get; set; }
        public string UserID { get; set; }
        public string ServiceIDs { get; set; }
        public Guid SlotID { get { return Guid.Parse(RowKey); } }

        public static explicit operator Slot(TableResult v)
        {
            DynamicTableEntity entity = (DynamicTableEntity)v.Result;
            Slot SlotProfile = new Slot();
            SlotProfile.PartitionKey = entity.PartitionKey;
            SlotProfile.RowKey = entity.RowKey;
            SlotProfile.Timestamp = entity.Timestamp;
            SlotProfile.ETag = entity.ETag;
            SlotProfile.Timeslot = entity.Properties["Timeslot"].DateTime;
            SlotProfile.UserID = entity.Properties["UserID"].StringValue;
            SlotProfile.ServiceIDs = entity.Properties["ServiceIDs"].StringValue;

            return SlotProfile;
        }

    }
}