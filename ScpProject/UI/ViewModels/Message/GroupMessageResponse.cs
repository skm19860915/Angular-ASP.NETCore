namespace Controllers.ViewModels.Message
{
    public class GroupMessageResponse
    {
        public string MessageContent { get; set; }
        public int MessageGroupId { get; set; }
        public int ParentMessageId { get; set; }
        public string MessageGroupTitle { get; set; }
    }
}