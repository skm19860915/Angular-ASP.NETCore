using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_ProgramDay
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int AssignedProgram_ProgramId { get; set; }

        [ForeignKey("AssignedProgram_ProgramId ")]
        public virtual AssignedProgram_Program ParentProgram { get; set; }
    }
}
