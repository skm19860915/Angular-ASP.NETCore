using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Athlete
{
    public class AthleteProgramHistory
    {
        public int Id { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int AssignedProgramId { get; set; }
        public int AthleteId { get; set; }
        public DateTime ArchivedDate { get; set; }
        public bool HideProgramOnHistoryPage { get; set; }
        public bool HideSurveyOnProgramHistoryPage { get; set; }
        //[ForeignKey("ProgramId")]
        //public virtual Program Program { get; set; }
        [ForeignKey("AthleteId")]
        public virtual Athlete Athlete { get; set; }
    }
}
