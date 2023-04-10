namespace DAL.DTOs.AthleteAssignedPrograms
{
    public class AthleteAssignedQuestions
    {
        public int ProgramDaySurveyItemId { get; set; }
        public int QuestionId { get; set; }
        public int QuestionTypeId { get; set; }
        public string QuestionDisplayText { get; set; }
        public string Answer{ get; set; }
        public int AssignedProgramId { get; set; }
        public int ProgramId { get; set; }
        public int DisplayWeekId { get; set; }

        public string SurveyName { get; set; }
        public int SurveyId { get; set; }
     }
}
