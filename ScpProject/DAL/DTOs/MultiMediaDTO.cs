
using e = Models.Enums;

namespace DAL.DTOs
{
    public class MultiMediaDTO
    {
        public int MultiMediaId { get; set; }
        public string Title { get; set; }
        public e.MediaType MultiMediaType { get; set; }
    }
}
