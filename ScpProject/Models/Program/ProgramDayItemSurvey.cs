using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Program
{
    public  class ProgramDayItemSurvey
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int ProgramDayItemId { get; set; }

        [ForeignKey("SurveyId")]
        public virtual Models.Survey.Survey Survey { get; set; }

        [ForeignKey("ProgramDayItemId")]
        public virtual ProgramDayItem TargetProgramDayItem { get; set; }
    }
}
