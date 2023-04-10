using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_SuperSetNote
    {
        public int Id { get; set; }
        public string Note { get; set; }
        public int AssignedProgram_ProgramDayItemSuperSetId { get; set; }
        public int Position { get; set; }

        [ForeignKey("AssignedProgram_ProgramDayItemSuperSetId")]
        public virtual AssignedProgram_ProgramDayItemSuperSet TargetProgramDayItemSuperSet { get; set; }
    }
    public class AssignedProgram_SuperSetNoteDisplayWeek
    {
        public int Id { get; set; }
        public int AssignedProgram_SuperSetNoteId { get; set; }
        public int DisplayWeek { get; set; }
        [ForeignKey("AssignedProgram_SuperSetNoteId")]
        public virtual AssignedProgram_SuperSetNote TargetNote { get; set; }
    }
}
