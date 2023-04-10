using System.Collections.Generic;
using b = BL.BusinessObjects;

namespace BL.BusinessObjects
{
    public class Exercise
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Notes { get; set; }
        public int CreatedUserId { get; set; }
        public bool IsDeleted { get; set; }
        public double? Percent { get; set; }
        public int? PercentMetricCalculationId { get; set; }
        public List<b.Tag> Tags { get; set; }
        public bool CanModify { get; set; }
        public string VideoURL { get; set; }
        public string CalcMetricName { get; set; }
    }
}
