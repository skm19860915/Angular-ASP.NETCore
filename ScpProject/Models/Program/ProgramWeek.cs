using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class ProgramWeek
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int ProgramDayItemExerciseId { get; set; }
        public virtual List<ProgramSet> SetsAndReps { get; set; }

        [ForeignKey("ProgramDayItemExerciseId")]
        public virtual ProgramDayItemExercise ParentProgramDayItemExercise { get; set; }

    }
}
