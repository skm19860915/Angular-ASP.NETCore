using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class ProgramDayItemExercise
    {
        public int Id { get; set; }
        public int ExerciseId { get; set; }
        public int WorkoutId { get; set; }
        public int ProgramDayItemId { get; set; }


        [ForeignKey("ExerciseId")]
        public virtual Exercise.Exercise TargetExercise { get; set; }
        [ForeignKey("ProgramDayItemId")]
        public virtual ProgramDayItem TargetProgramDayItem { get; set; }
    }
}
