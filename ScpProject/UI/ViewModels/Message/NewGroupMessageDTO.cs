using System.Collections.Generic;

namespace Controllers.ViewModels.Message
{
    public class NewGroupMessageDTO
    {
        public string GroupTitle { get; set; }
        public int ParentMessageId { get; set; }
        public string MessageContent { get; set; }
        public List<int> UsersToSendTo { get; set; }
        public bool ReadOnly { get; set; }
        public bool Pause { get; set; }
    }
}