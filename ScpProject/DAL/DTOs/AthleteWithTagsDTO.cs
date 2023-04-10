using System.Collections.Generic;

namespace DAL.DTOs
{
    public class AthleteWithTagsDTO
    {
        public int AthleteId { get; set; }
        public List<TagDTO> Tags { get; set; }
    }
}
