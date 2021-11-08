using System;

namespace Rasputin.TM {
    public class CreateSlotRequest {
        public DateTime Timeslot {get;set;}
        public Guid UserID {get;set;}
        public Guid[] ServiceIDs {get;set;}
    }
}