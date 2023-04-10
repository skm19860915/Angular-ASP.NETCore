using System;

namespace Models.Administration
{
    public class FeedBack
    {
        public int Id { get; set; }
        public string feedBack { get; set; }
        public int UserId { get; set; }
        public DateTime Sent { get; set; }
    }
}
