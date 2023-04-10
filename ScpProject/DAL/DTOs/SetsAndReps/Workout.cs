using System;
using System.Collections.Generic;

namespace DAL.DTOs.SetsAndReps
{
    public class Workout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public String Notes { get; set; }
        public List<TagDTO> Tags { get; set; }
        public Boolean CanModify { get; set; }
        public Boolean IsDeleted { get; set; }
        public string Rest { get; set; }
        public bool ShowWeight { get; set; }
    }
}
