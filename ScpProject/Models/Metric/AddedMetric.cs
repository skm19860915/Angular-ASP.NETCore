using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.Metric
{
    public class AddedMetric
    {
        public int Id { get; set; }
        public int MetricId { get; set; }
        public double Value { get; set; }
        public DateTime CompletedDate { get; set; }
        public int AthleteId { get; set; }
        public int EnteredByUserId { get; set; }

        [ForeignKey("MetricId")]
        public virtual Metric Metric { get; set; }
        [ForeignKey("AthleteId")]
        public virtual Athlete.Athlete Athlete { get; set; }
        [ForeignKey("EnteredByUserId")]
        public virtual User.User EnteredByUser { get; set; }

    }
}
