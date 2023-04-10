using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Messages
{
    [Table("MessagesToUsersInGroups")]
    public class MessageToUserInGroup
    {
        public int Id { get; set; }
        public int DestinationUserId { get; set; }
        public int MessageId { get; set; }
        public int MessageGroupId { get; set; }
        public DateTime? ReadTime { get; set; }
        [ForeignKey("MessageId")]
        public virtual Message SentMessage { get; set; }
        [ForeignKey("DestinationUserId")]
        public virtual User.User DestinationUser { get; set; }
    }
}
