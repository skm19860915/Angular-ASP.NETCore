using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public class CompletedAssignedProgramDay
    {
        public int Id { get; set; }
        public int AssignedProgramId { get; set; }
        public int ProgramDayId { get; set; }
        public int AthleteId { get; set; }
        public int WeekNumber { get; set; }

        [ForeignKey("AssignedProgramId")]
        public virtual AssignedProgram AssignedProgram { get; set; }
        [ForeignKey("ProgramDayId")]
        public virtual ProgramDay ProgramDay { get; set; }
        [ForeignKey("AthleteId")]
        public virtual Athlete.Athlete TargetAthlete { get; set; }


    }
}
