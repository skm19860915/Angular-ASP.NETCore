using System;
using System.Collections.Generic;

namespace DAL.DTOs.Metrics
{
    public class AthleteCompletedMetricHomePage
    {
        public List<AthleteCompletedMeasurementById> CompletedMetrics { get; set; }
    }
    public class AthleteCompletedMeasurementById
    {
        public int UnitOfMeasurementId { get; set; }
        public string UnitOfMeasurementName { get; set; }
        public List<AthleteCompletedMetric> Metrics { get; set; }
    }
    public class AthleteCompletedMetric
    {
        public double MetricValue { get; set; }
        public DateTime CompletedDate { get; set; }
        public int MetricId { get; set; }
        public string MetricName { get; set; }
        public int UnitOfMeasurementId { get; set; }
        public string UnitOfMeasurementName { get; set; }
        public string CompletedDateDisplay { get; set; }
    }
}
