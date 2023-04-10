using System.Collections.Generic;

namespace DAL.DTOs.SetsAndReps
{
    public class WorkoutWithTagsDTO
    {
        public int WorkoutId { get; set; }
        public List<TagDTO> Tags { get; set; }
    }
}
