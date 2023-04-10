using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models.Athlete
{
    public class CompletedQuestionOpenEnded
    {
        public int Id { get; set; }
        public string Response { get; set; }//right now this is the easiest way. 
        public int AthleteId { get; set; }
        public int QuestionId { get; set; }
        public DateTime CompletedDate { get; set; }
        public int AssignedProgramId { get; set; }
        public int ProgramDayItemSurveyId { get; set; }
        public int WeekId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Survey.Question Question { get; set; }
        [ForeignKey("AthleteId")]
        public virtual Athlete TargetAthlete { get; set; }
        [ForeignKey("AssignedProgramId")]
        public Program.AssignedProgram AssignedProgram { get; set; }
        [ForeignKey("ProgramDayItemSurveyId")]
        public Program.ProgramDayItemSurvey ProgramDayItemSurvey { get; set; }
    }


}
