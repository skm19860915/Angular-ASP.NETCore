using System;
using System.Collections.Generic;
using t = Controllers.ViewModels.Tag;

namespace Controllers.ViewModels.SetAndRep
{
    public class WorkoutDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public String Notes { get; set; }
        public DateTime CreatedDate { get; set; }
        public int CreatedUserId { get; set; }
        public virtual List<Week> TotalWorkout { get; set; }
        public List<t.Tag> Tags { get; set; }
        public string Rest { get; set; }

        public bool ShowWeight { get; set; }

    }
}