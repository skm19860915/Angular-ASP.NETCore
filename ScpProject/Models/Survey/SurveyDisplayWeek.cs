using Models.Program;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Survey
{
   public class SurveyDisplayWeek
    {
        public int Id { get; set; }
        public int ProgramDayItemSurveyId { get; set; }
        public int DisplayWeek { get; set; }

        [ForeignKey("ProgramDayItemSurveyId")]
        public virtual ProgramDayItemSurvey ProgramDayItemSurvey { get; set; }
    }
}
