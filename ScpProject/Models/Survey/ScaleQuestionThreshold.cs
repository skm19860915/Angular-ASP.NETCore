using Models.Enums;
namespace Models.Survey
{
    public class ScaleQuestionThreshold
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public QuestionThresholdEnum Comparer { get; set; }
        public int ThresholdValue { get; set; }
    }
}
