using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
  public  class AssignedProgram_ProgramDayItemSuperSetWeek
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int AssignedProgram_SuperSetExerciseId { get; set; }
        public virtual List<AssignedProgram_ProgramDayItemSuperSet_Set> SetsAndReps { get; set; }

        [ForeignKey("AssignedProgram_SuperSetExerciseId")]
        public virtual AssignedProgram_SuperSetExercise SuperSetExercise { get; set; }
    }
}
