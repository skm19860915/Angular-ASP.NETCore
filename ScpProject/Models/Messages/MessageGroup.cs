using System;

namespace Models.Messages
{
    public class MessageGroup
    {
        public int Id { get; set; }
        public string GroupTitle { get; set; }
        public int CreatedUserId { get; set; }
        public Guid SignalRGroupId { get; set; }
    }
}
