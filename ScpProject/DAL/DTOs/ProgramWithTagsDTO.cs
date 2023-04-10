using System.Collections.Generic;

namespace DAL.DTOs
{
    public class ProgramWithTagsDTO
    {
        public int ProgramId { get; set; }
        public List<TagDTO> Tags { get; set; }
    }
}
