using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_NoteDisplayWeek
    {
        public int Id { get; set; }
        public int AssignedProgram_ProgramDayItemNoteId { get; set; }
        public int DisplayWeek { get; set; }

        [ForeignKey("AssignedProgram_ProgramDayItemNoteId")]
        public virtual AssignedProgram_ProgramDayItemNote Note { get; set; }
    }
}
