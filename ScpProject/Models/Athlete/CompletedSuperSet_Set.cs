using System.ComponentModel.DataAnnotations.Schema;
using System;


namespace Models.Athlete
{
    public class CompletedSuperSet_Set
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Percent { get; set; }
        public double Weight { get; set; }
        public int RepsAchieved { get; set; }
        public int OriginalSuperSet_SetId { get; set; }
        public int AthleteId { get; set; }
        public int AssignedProgramId { get; set; }
        public DateTime CompletedDate { get; set; }
        public int CompletedRepsAchieved { get; set; }
        [ForeignKey("OriginalSuperSet_SetId")]
        public virtual Program.ProgramDayItemSuperSet_Set OriginalSuperSet_Set { get; set; }

        [ForeignKey("AthleteId")]
        public virtual Athlete TargetAthlete { get; set; }
        [ForeignKey("AssignedProgramId")]
        public Program.AssignedProgram AssignedProgram { get; set; }
    }
}
