using System;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Athlete;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_ProgramHistory
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public int AssignedProgram_ProgramId { get; set; }
        public DateTime InsertedTime { get; set; }

        [ForeignKey("AssignedProgram_ProgramId")]
        public virtual AssignedProgram_Program PastProgram {get;set;}

        [ForeignKey("AthleteId")]
        public virtual Athlete.Athlete TargetAthlete { get; set; }
    }
}
