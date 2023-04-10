using System;

namespace Models.Messages
{

    public class Message
    {
        public int Id { get; set; }
        public int? ParentMessageId { get; set; }
        public string Content { get; set; }
        public DateTime SentTime { get; set; }
        public int CreatedUserId { get; set; }
        public bool ReadOnly { get; set; }
        public bool Pause { get; set; }

    }
}
