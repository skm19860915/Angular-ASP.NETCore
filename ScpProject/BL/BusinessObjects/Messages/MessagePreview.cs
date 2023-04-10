using System.Collections.Generic;
using DAL.DTOs.Messages;

namespace BL.BusinessObjects.Messages
{
    public class MessagePreview
    {
        public List<GroupMessagePreviewDTO> GroupUserMessages { get; set; }
        public List<UserMessagePreviewDTO> UserMessages{ get; set; }
        public int TotalMessageCount { get; set; }
    }
}
