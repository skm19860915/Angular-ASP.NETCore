using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class SuperSetExercise
    {
        public int Id { get; set; }
        public int ProgramDayItemSuperSetId { get; set; }
        public int Position { get; set; }
        public int ExerciseId { get; set; }
        public string Rest { get; set; }
        public bool ShowWeight { get; set; }


        [ForeignKey("ExerciseId")]
        public virtual Exercise.Exercise TargetExercise { get; set; }

        [ForeignKey("ProgramDayItemSuperSetId")]
        public virtual ProgramDayItemSuperSet TargetProgramDayItemSuperSet { get; set; }
    }
}
