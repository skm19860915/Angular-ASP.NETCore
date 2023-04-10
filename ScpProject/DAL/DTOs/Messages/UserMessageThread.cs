using System;

namespace DAL.DTOs.Messages
{
    public class MessageThread
    {
        public int MessageId { get; set; }
        public string MessageContent { get; set; }
        public DateTime SentTime { get; set; }
        public int CreatedUserId { get; set; }
        public String UserFirstName { get; set; }
        public String UserLastName { get; set; }
        public int ViewerId { get; set; }
    }
}
