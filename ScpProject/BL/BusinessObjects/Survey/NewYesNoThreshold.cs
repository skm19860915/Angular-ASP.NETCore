using System.Collections.Generic;

namespace BL.BusinessObjects.Survey
{
    public class NewYesNoThreshold
    {
        public int QuestionId { get; set; }
        public bool ThresholdValue { get; set; }
        public List<int> CoachIds { get; set; }
    }
}
