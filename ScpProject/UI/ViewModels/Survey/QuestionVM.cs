using System.Collections.Generic;
using Models.Enums;
using BL.BusinessObjects.Survey;
namespace Controllers.ViewModels.Survey
{
    public class QuestionVM
    {
        public int SurveysToQuestionsId { get; set; }
        public int QuestionId { get; set; }
        public string Question { get; set; }
        public QuestionTypeEnum QuestionType { get; set; }
        public int SurveyId { get; set; }
        public List<NewYesNoThreshold> YesNoThresholds { get; set; }
        public List<NewScaleThreshold> ScaleThresholds { get; set; }
        public bool CanModify { get; set; }
    }
}