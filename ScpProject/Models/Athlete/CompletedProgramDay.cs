using Models.Program;
using System.ComponentModel.DataAnnotations.Schema;
using System;

namespace Models.Athlete
{
    /// <summary>
    /// by virtue of having a record in this table they have already completed said day, no need to have an is completed flag.
    /// </summary>
    public class CompletedProgramDay
    {
        public int Id { get; set; }
        public int AthleteId { get; set; }
        public int AssignedProgramId { get; set; }
        public int ProgramDayId { get; set; }
        public DateTime CompletedDate { get; set; }
        public int WeekId { get; set; }
        [ForeignKey("AthleteId")]
        public virtual Athlete Athlete { get; set; }
        [ForeignKey("AssignedProgramId")]
        public virtual AssignedProgram AssignedProgram { get; set; }
        [ForeignKey("ProgramDayId")]
        public virtual ProgramDay ProgramDay { get; set; }

    }
}
