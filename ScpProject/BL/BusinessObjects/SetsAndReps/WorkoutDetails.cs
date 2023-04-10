using System;
using System.Collections.Generic;

namespace BL.BusinessObjects.SetsAndReps
{
    public class WorkoutDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public String Notes { get; set; }
        public int CreatedUserId { get; set; }
        public string Rest { get; set; }
        public bool ShowWeight { get; set; }
        public virtual List<Week> TotalWorkout { get; set; }
        public List<Tag> Tags { get; set; }
        public Boolean CanModify { get; set; }
        public Boolean IsDeleted { get; set; }
    }
}
