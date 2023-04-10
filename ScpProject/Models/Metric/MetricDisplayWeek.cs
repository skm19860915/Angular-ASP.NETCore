using Models.Program;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Metric
{
    public class MetricDisplayWeek
    {
        public int Id { get; set; }
        public int ProgramDayItemMetricId { get; set; }
        public int DisplayWeek { get; set; }
        [ForeignKey("ProgramDayItemMetricId")]
        public virtual ProgramDayItemMetric ProgramDayItemMetric { get; set; }

    }
}
