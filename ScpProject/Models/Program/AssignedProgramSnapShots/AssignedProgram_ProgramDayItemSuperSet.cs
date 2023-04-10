using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_ProgramDayItemSuperSet
    {
        public int Id { get; set; }
        public int AssignedProgram_ProgramDayItemId { get; set; }

        public string SuperSetDisplayTitle { get; set; }

        [ForeignKey("AssignedProgram_ProgramDayItemId")]
        public virtual AssignedProgram_ProgramDayItem TargetAssignedProgram_ProgramDayItem { get; set; }
    }
}
