using System;
using System.Collections.Generic;
using t = Controllers.ViewModels.Tag;

namespace Controllers.ViewModels.Exercise
{
    public class ExerciseVM
    {
        public int Id { get; set; }
        public string Notes { get; set; }
        public string Name { get; set; }
        public int CreatedUserId { get; set; }
        public Boolean IsDeleted { get; set; }
        public double? Percent { get; set; }
        public int? PercentMetricCalculationId { get; set; }
        public List<t.Tag> Tags { get; set; }
        public string VideoURL { get; set; }
    }
}