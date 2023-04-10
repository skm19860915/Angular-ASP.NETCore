using System.Collections.Generic;
using e = Models.Enums;

namespace DAL.DTOs
{
    public class MultiMediaWithTagsDTO
    {
        public int MultiMediaId { get; set; }
        public List<TagDTO> Tags { get; set; }
        public e.MediaTypeEnum MultiMediaType { get; set; }
    }
}
