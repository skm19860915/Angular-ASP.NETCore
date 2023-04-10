using System;

namespace DAL.DTOs.Survey
{
    public class PastSurveyItem
    {
        public int AssignedProgramId { get; set; }
        public string SurveyName { get; set; }
        public DateTime? SurveyCompleted { get; set; }
        public string ProgramName { get; set; }
        public int IsSnapShot => 0;
    }
}
