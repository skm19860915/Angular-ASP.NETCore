using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_AssignedProgramHistory
    {
        [Index("IX_AssignedProgram", 1, IsUnique = true)]
        public int Id { get; set; }
        [Index("IX_AssignedProgram", 2, IsUnique = true)]
        public DateTime AssignedDate { get; set; }
        public DateTime CompletedDate { get; set; }
        public int AssignedProgram_ProgramId { get; set; }
        public int AthleteId { get; set; }

        [ForeignKey("AssignedProgram_ProgramId")]
        public virtual AssignedProgram_Program TargetProgram { get; set; }

        [ForeignKey("AthleteId")]
        public virtual Athlete.Athlete TagetAthlete { get; set; }
    }
}
