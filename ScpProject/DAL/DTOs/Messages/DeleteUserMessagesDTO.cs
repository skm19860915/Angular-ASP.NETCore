namespace DAL.DTOs.Messages
{
    public class DeleteUserMessagesDTO
    {
        public int DestinationUserId { get; set; }
        public int CreatedUserId { get; set; }
    }
}
