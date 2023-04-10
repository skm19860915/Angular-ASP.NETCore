using System;

namespace DAL.DTOs.Metrics
{
    public class CompletedMetricDisplay
    {
        public int MetricId { get; set; }
        public string Name { get; set; }
        public DateTime CompletedDate { get; set; }
        public double Value { get; set; }
    }
}
