using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Messages
{
    [Table("MessagesToUsers")]
    public class MessageToUser
    {
        public int Id { get; set; }
        /// <summary>
        /// messages have three cases
        /// 1. Athlete -> Athlete
        /// 2. Coach -> Athlete
        /// 3. Coach -> Coach
        /// hence why we are going off of userId 
        /// </summary>
        public int DestinationUserId { get; set; }
        public DateTime? ReadTime { get; set; }
        public int MessageId { get; set; }
        [ForeignKey("MessageId")]
        public virtual Message SentMessage { get; set; }
        [ForeignKey("DestinationUserId")]
        public virtual User.User DestinationUser { get; set; }
    }
}
