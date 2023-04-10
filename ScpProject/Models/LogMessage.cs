using System;

namespace Models
{
    public class LogMessage
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string StackTrace { get; set; }
        public int UserId { get; set; }
        public DateTime LoggedDate { get; set; }
    }
}
