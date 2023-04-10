using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class ProgramSet
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public int Percent { get; set; }
        public int Weight { get; set; }
        public int ParentProgramWeekId { get; set; }
        [ForeignKey("ParentProgramWeekId")]
        public virtual ProgramWeek ParentProgramWeek { get; set; }
    }
}
