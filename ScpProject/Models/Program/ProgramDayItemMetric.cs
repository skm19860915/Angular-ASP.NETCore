using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
   public class ProgramDayItemMetric
    {
        public int Id { get; set; }
        public int MetricId { get; set; }
        public int ProgramDayItemId { get; set; }

        [ForeignKey("MetricId")]
        public virtual Models.Metric.Metric Survey { get; set; }

        [ForeignKey("ProgramDayItemId")]
        public virtual ProgramDayItem TargetProgramDayItem { get; set; }
    }
}
