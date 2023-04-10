using Models.Program;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Athlete
{
    public class CompletedProgramWeek
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public int AssignedProgramId { get; set; }
        public int ProgramWeek { get; set; }
        [ForeignKey("AthleteId")]
        public virtual Athlete Athlete { get; set; }
        [ForeignKey("AssignedProgramId")]
        public virtual AssignedProgram AssignedProgram { get; set; }
        
    }
}
