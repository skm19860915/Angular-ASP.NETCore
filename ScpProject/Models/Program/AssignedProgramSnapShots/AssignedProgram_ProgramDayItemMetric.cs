using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_ProgramDayItemMetric
    {
        public int Id { get; set; }
        public int MetricId { get; set; }

        public int AssignedProgram_ProgramDayItemId { get; set; }

        [ForeignKey("MetricId")]
        public virtual Metric.Metric Survey { get; set; }

        [ForeignKey("AssignedProgram_ProgramDayItemId")]
        public virtual AssignedProgram_ProgramDayItem TargetProgramDayItem { get; set; }
    }
}
