using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_ProgramDayItem
    {

        public int Id { get; set; }
        public int AssignedProgram_ProgramDayId { get; set; }
        public int Position { get; set; }
        public Models.Enums.ProgramDayItemEnum ItemEnum { get; set; }

        [ForeignKey("AssignedProgram_ProgramDayId")]
        public virtual AssignedProgram_ProgramDay AssignedProgram_ParentProgramDay { get; set; }
    }
}
