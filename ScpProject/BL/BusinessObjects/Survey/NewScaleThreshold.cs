using System.Collections.Generic;

namespace BL.BusinessObjects.Survey
{
    public class NewScaleThreshold
    {
        public int QuestionId { get; set; }
        public Models.Enums.QuestionThresholdEnum  Comparer{ get; set; }
        public int ThresholdValue { get; set; }
        public List<int> CoachIds { get; set; }
    }
}
