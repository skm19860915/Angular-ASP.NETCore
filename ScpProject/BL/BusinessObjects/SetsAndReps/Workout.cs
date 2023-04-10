using System;
using System.Collections.Generic;

namespace BL.BusinessObjects.SetsAndReps
{
    public class Workout
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public String Notes { get; set; }
        public List<Tag> Tags { get; set; }
        public Boolean CanModify { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}
