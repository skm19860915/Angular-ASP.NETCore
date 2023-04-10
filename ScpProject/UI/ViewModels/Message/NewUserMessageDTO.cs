namespace Controllers.ViewModels.Message
{
    public class NewUserMessageDTO
    {
        public int ParentMessageId { get; set; }
        public string MessageContent { get; set; }
        public int UserToSendTo { get; set; }
        public bool ReadOnly { get; set; }
        public bool Pause { get; set; }
    }
}