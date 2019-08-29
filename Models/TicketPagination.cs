using System;

namespace QB.Models
{
    public class TicketPagination
    {
        public long Npp { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string Customer { get; set; }
        public string CustomerNpp { get; set; }
        public string State { get; set; }
        public string StateName { get; set; }
        public string Priority { get; set; }
        public string PriorityName { get; set; }
        public string PriorityColor { get; set; }
        public string Agent { get; set; }
        public string CreateTime { get; set; }
        public long RowNum { get; set; }
        public long MaxRow { get; set; }
        public string LastUpdate { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
    }
}
