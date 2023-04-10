using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class ProgramDayItemSuperSetWeek
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int SuperSetExerciseId { get; set; }
        public virtual List<ProgramDayItemSuperSet_Set> SetsAndReps { get; set; }

        [ForeignKey("SuperSetExerciseId")]
        public virtual SuperSetExercise SuperSetExercise { get; set; }
    }
}
