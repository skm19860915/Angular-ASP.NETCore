using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_MetricsDisplayWeek
    {
        public int Id { get; set; }
        public int AssignedProgram_ProgramDayItemMetricId { get; set; }
        public int DisplayWeek { get; set; }
        public double? Value { get; set; }
        public DateTime? CompletedDate { get; set; }
        [ForeignKey("AssignedProgram_ProgramDayItemMetricId ")]
        public virtual AssignedProgram_ProgramDayItemMetric ProgramDayItemMetric { get; set; }
    }
}
