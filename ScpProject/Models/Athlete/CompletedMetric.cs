using System;
using System.ComponentModel.DataAnnotations.Schema;
namespace Models.Athlete
{
    public class CompletedMetric
    {
        public int Id { get; set; }
        public double Value { get; set; }
        public DateTime CompletedDate { get; set; }
        public int AssignedProgramId { get; set; }
        public int AthleteId { get; set; }
        public int MetricId { get; set; }
        public int ProgramDayItemMetricId { get; set; }
        public int WeekId { get; set; }
  
        [ForeignKey("MetricId")]
        public virtual Metric.Metric Metric { get; set; }
        [ForeignKey("AthleteId")]
        public virtual Athlete TargetAthlete { get; set; }
        [ForeignKey("AssignedProgramId")]
        public Program.AssignedProgram AssignedProgram { get; set; }
        [ForeignKey("ProgramDayItemMetricId")]
        public Program.ProgramDayItemMetric ProgramDayItemMetric { get;set; }
    }
}
 