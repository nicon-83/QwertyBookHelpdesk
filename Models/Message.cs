using System;

namespace QB.Models
{
    public class Message
    {
        public long TicketNpp { get; set; }
        public string TicketNumber { get; set; }
        public string Priority { get; set; }
        public string AgentNpp { get; set; }
        public string Agent { get; set; }
        public string State { get; set; }
        public string Text { get; set; }
        public string CreateTime { get; set; }
        public long MessageNpp { get; set; }
        public string MessageClient { get; set; }
        public string MessageAgent { get; set; }
        public string TicketTitle { get; set; }
        public string UserId { get; set; }
        public string UserFullName { get; set; }
        public string AttachmentsData { get; set; }
        public string Emails { get; set; }
    }
}
