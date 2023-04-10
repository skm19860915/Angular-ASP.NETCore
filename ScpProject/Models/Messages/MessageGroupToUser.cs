using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Messages
{
    [Table("MessageGroupsToUsers")]
    public class MessageGroupToUser
    {
        public int Id { get; set; }
        public int MessageGroupId { get; set; }
        /// <summary>
        /// messages have three cases
        /// 1. Athlete -> Athlete
        /// 2. Coach -> Athlete
        /// 3. Coach -> Coach
        /// hence why we are going off of userId 
        /// </summary>
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User.User DestinationUser { get; set; }
        [ForeignKey("MessageGroupId")]
        public virtual MessageGroup Group { get; set; }
    }
}
