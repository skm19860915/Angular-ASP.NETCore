using System.Collections.Generic;
using b = BL.BusinessObjects;

namespace BL.BusinessObjects.Metric
{
    public class MetricDetails
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? UnitOfMeasurementId { get; set; }
        public string UnitOfMeasurementType { get; set; }
        public List<b.Tag> Tags { get; set; }
        public bool CanModify { get; set; }
        public bool IsDeleted { get; set; }
    }
}
