using System;

namespace DAL.DTOs.Messages
{
    public class UserMessagePreviewDTO
    {
        public int LastSentByUserId { get; set; }
        public int MessageId { get; set; }
        public string LastSentFirstName { get; set; }
        public string LastSentLastName { get; set; }
        public int UserId { get; set; } //used to say if itwas from an athlete or a coach
        public int AthleteId { get; set; }  //used to say if itwas from an athlete or a coach
        public string MessageContent { get; set; }
        public DateTime SentTime { get; set; }
        public int ViewerId { get; set; }
        public int DestinationUserId { get; set; }
        public bool ReadOnly { get; set; }
        public bool Pause { get; set; }
        public string DestinationFirstName { get; set; }
        public string DestinationLastName { get; set; }
        public DateTime? ReadTime { get; set; }
        public Guid SignalRGroupId { get; set; }
    }
}
