using System;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Athlete;
using Models.Survey;

namespace Models.Program.AssignedProgramSnapShots
{
    public class AssignedProgram_CompletedQuestionYesNo
    {
        public int Id { get; set; }
        public bool YesNoValue { get; set; }//right now this is the easiest way. 
        public int AthleteId { get; set; }
        public int QuestionId { get; set; }
        public DateTime CompletedDate { get; set; }
        public int AssignedProgram_ProgramId { get; set; }
        public int AssignedProgram_ProgramDayItemSurveyId { get; set; }
        public int WeekId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Survey.Question Question { get; set; }
        [ForeignKey("AthleteId")]
        public virtual Athlete.Athlete TargetAthlete { get; set; }
        [ForeignKey("AssignedProgram_ProgramId")]
        public AssignedProgram_Program AssigAssignedProgram_AssignedProgramIdnedProgram { get; set; }
        [ForeignKey("AssignedProgram_ProgramDayItemSurveyId")]
        public AssignedProgram_ProgramDayItemSurvey ProgramDayItemSurvey { get; set; }
    }


}
