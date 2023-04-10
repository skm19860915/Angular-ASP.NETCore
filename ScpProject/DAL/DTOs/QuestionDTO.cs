namespace DAL.DTOs
{
    public class QuestionDTO
    {
        public int SurveysToQuestionsId { get; set; }
        public string QuestionDisplayText { get; set; }
        public int QuestionTypeId { get; set; }
        public int Id { get; set; }
        public int SurveyId { get; set; }
        public bool CanModify { get; set; }
    }
}
