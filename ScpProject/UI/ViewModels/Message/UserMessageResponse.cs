namespace Controllers.ViewModels.Message
{
    public class UserMessageResponse
    {
        public string MessageContent { get; set; }
        public int DestinationUserId { get; set; }
        public int ParentMessageId { get; set; }
    }
}