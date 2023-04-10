namespace DAL.DTOs.Messages
{
    public class PauseMessageDTO
    {
        public bool PauseValue { get; set; }
        public int ParentMessageId { get; set; }
    }
}
