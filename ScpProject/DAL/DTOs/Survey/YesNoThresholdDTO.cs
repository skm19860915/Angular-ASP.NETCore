using System.Collections.Generic;

namespace DAL.DTOs.Survey
{
    public class YesNoThresholdDTO
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public bool Threshold { get; set; }
        public List<int> CoachIds { get; set; }
    }
}
