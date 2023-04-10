using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
   public class AssignedProgram_MovieDisplayWeek
    {
        public int Id { get; set; }
        public int AssignedProgram_ProgramDayItemMovieId { get; set; }
        public int DisplayWeek { get; set; }
        [ForeignKey("AssignedProgram_ProgramDayItemMovieId")]
        public virtual AssignedProgram_ProgramDayItemMovie ProgramDayItemMovie { get; set; }
    }
}
