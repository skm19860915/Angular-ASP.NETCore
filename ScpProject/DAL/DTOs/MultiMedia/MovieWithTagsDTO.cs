using System.Collections.Generic;

namespace DAL.DTOs.MultiMedia
{
    public class MovieWithTagsDTO
    {
        public int MovieId { get; set; }
        public List<TagDTO> Tags { get; set; }
    }
}
