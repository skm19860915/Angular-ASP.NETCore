using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Metric
{
    [Table("TagsToMetrics")]
    public class TagToMetric
    {
        public int Id { get; set; }
        public int TagId { get; set; }
        public int MetricId { get; set; }

        [ForeignKey("TagId")]
        public virtual MetricTag Tag { get; set; }

        [ForeignKey("MetricId")]
        public virtual Metric TaggedMetric { get; set; }
    }
}
