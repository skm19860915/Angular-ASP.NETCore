using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
   public class AssignedProgram_ProgramDayItemMovie
    {
        public int Id { get; set; }
        public int MovieId { get; set; }
        public int AssignedProgram_ProgramDayItemId { get; set; }

        [ForeignKey("MovieId")]
        public virtual Models.MultiMedia.Movie Movie { get; set; }
        [ForeignKey("AssignedProgram_ProgramDayItemId")]
        public virtual AssignedProgram_ProgramDayItem TargetProgramDayItem { get; set; }
    }
}
