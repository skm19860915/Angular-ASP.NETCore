using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Survey
{
    [Table("SurveysToQuestions")]
    public class SurveysToQuestions
    {
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public int QuestionId { get; set; }

        [ForeignKey("SurveyId")]
        public virtual Survey Survey { get; set; }
        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}
