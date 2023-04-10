using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Athlete
{
    public class PastAthleteAssignedProgram
    {
        public int Id { get; set; }
        public int AssignedProgramId { get; set; }
        public int AthleteId { get; set; }
        public DateTime? ProgramStartDate { get; set; }
        public DateTime? ProgramEndDate { get; set; }

        [ForeignKey("AssignedProgramId")]
        public Program.AssignedProgram AssignedProgram { get; set; }

        [ForeignKey("AthleteId")]
        public Athlete AssignedAthlete { get; set; }
    }
}
