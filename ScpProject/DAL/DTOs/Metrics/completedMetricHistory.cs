using System;

namespace DAL.DTOs.Metrics
{
    public class CompletedMetricHistory
    {
        public int MetricId { get; set; }
        public string Name { get; set; }
        public double Value { get; set; }
        public DateTime CompletedDate { get; set; }
        public bool IsCompletedMetric { get; set; }
        public int Id { get; set; }
    }
}
