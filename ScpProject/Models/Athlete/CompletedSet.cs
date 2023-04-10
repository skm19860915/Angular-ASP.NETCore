
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Models.Athlete
{
    public class CompletedSet
    {
        public int Id { get; set; }
        public int Position { get; set; }
        public int Sets { get; set; }
        public int Reps { get; set; }
        public double Percent { get; set; }
        public double Weight { get; set; }
        public int OriginalSetId { get; set; }
        public int AthleteId { get; set; }
        public int AssignedProgramId { get; set; }
        public DateTime CompletedDate { get; set; }
        [ForeignKey("OriginalSetId")]
        public virtual Program.ProgramSet OriginalSet { get; set; }

        [ForeignKey("AthleteId")]
        public virtual Athlete TargetAthlete { get; set; }
        [ForeignKey("AssignedProgramId")]
        public Program.AssignedProgram AssignedProgram { get; set; }
    }
}
