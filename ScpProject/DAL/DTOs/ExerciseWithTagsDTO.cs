using System.Collections.Generic;

namespace DAL.DTOs
{
    public class ExerciseWithTagsDTO
    {
        public int ExerciseId { get; set; }
        public List<TagDTO> Tags { get; set; }
    }
}
