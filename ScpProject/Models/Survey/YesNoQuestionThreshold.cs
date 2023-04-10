namespace Models.Survey
{
    public class YesNoQuestionThreshold
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public bool Threshold { get; set; }
    }
}
