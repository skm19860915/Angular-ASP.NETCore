using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using BL;
using b = BL.BusinessObjects;
using Controllers.ViewModels.Survey;
using BL.CustomExceptions;
using DAL.DTOs.AthleteAssignedPrograms;
using System.Web.Configuration;
using DAL.DTOs.Survey;
using Models.Survey;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Survey")]
    public class SurveyController : ApiController
    {
        private ISurveyManager _surveyMan;
        private IProgramManager _programManager;
        public SurveyController(ISurveyManager survMan, IProgramManager progMan)
        {
            _surveyMan = survMan;
            _programManager = progMan;
        }
        [HttpPost, Route("AddScaleThreshold")]
        public void AddScaleThreshold(b.Survey.NewScaleThreshold newScaleThreshold)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.AddScaleThreshold(newScaleThreshold, userGuid);

        }
        [HttpPost, Route("AddYesNoThreshold")]
        public void AddyesNoThreshold(b.Survey.NewYesNoThreshold targetYesNoThreshold)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.AddYesNoThreshold(targetYesNoThreshold, userGuid);
        }
        [HttpPost, Route("DeleteYesNoThreshold/{thresholdId:int}")]
        public void DeleteYesNoThreshold([FromUri] int thresholdId, [FromBody] b.Survey.NewYesNoThreshold newYesNoThreshold)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.DeleteYesNoThreshold(newYesNoThreshold, userGuid, thresholdId);
        }
        [HttpPost, Route("DeleteScaleThreshold/{thresholdId:int}")]
        public void DeleteScaleThreshold([FromUri]int thresholdId, [FromBody] b.Survey.NewScaleThreshold newScaleThreshold)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.DeleteScaleThreshold(newScaleThreshold, userGuid, thresholdId);
        }

        [HttpPost, Route("UpdateScaleThreshold/{thresholdId:int}")]
        public void UpdateScaleThreshold([FromUri]int thresholdId, [FromBody] b.Survey.NewScaleThreshold newScaleThreshold)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.UpdateScaleThreshold(newScaleThreshold, userGuid, thresholdId);
        }
        [HttpPost, Route("UpateYesNoThreshold/{thresholdId:int}")]
        public void UpateYesNoThreshold([FromUri] int thresholdId, [FromBody] b.Survey.NewYesNoThreshold newYesNoThreshold)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.UpdateYesNoThreshold(newYesNoThreshold, userGuid, thresholdId);
        }
        [HttpGet, Route("HardDelete/{id:int}")]
        public void HardDelete(int id)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.HardDelete(id, userGuid);
        }

        [HttpGet, Route("GetPastSurveyList/{athleteId:int}")]
        public List<PastSurveyItem> GetPastSurveyList(int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _surveyMan.GetPastSurveyList(athleteId, userGuid);
        }
        [HttpPost, Route("DuplicateSurvey/{surveyId:int}")]
        public void DuplicateSurvey(int surveyId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.DuplicateSurvey(surveyId, userGuid);
        }
        [HttpPost, Route("ArchiveSurvey/{surveyId:int}")]
        public void ArchiveSurvey(int surveyId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.ArchiveSurvey(surveyId, userGuid);
        }
        [HttpPost, Route("UnArchiveSurvey/{surveyId:int}")]
        public void UnArchiveSurvey(int surveyId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.UnArchiveSurvey(surveyId, userGuid);
        }
        [HttpPost, Route("Create")]
        public HttpResponseMessage CreateSurvey(SurveyVM newSurvey)
        {

            var tagIds = !newSurvey.Tags.Any() ? new List<SurveyTag>() : newSurvey.Tags.Select(x => new SurveyTag() { Name = x.Name, Id = x.Id }).ToList();
            try
            {
                var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
                return Request.CreateResponse(HttpStatusCode.OK, _surveyMan.CreateNewSurvey(newSurvey.Name, newSurvey.Description, tagIds, userGuid));
            }
            catch (ItemAlreadyExistsException iex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, iex.Message);
            }
            catch (ItemValidationError ex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, ex.Message);
            }
        }
        [HttpGet, Route("GetAllSurveys")]
        public List<b.Survey.Survey> GetAllSurveys()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _surveyMan.GetAllSurveys(userGuid);
        }
        [HttpPost, Route("CreateQuestion")]
        public HttpResponseMessage CreateQuestion(QuestionVM newQuestion)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            try
            {
                return Request.CreateResponse(HttpStatusCode.OK, _surveyMan.CreateQuestion(newQuestion.Question, newQuestion.QuestionType, newQuestion.SurveyId, userGuid, newQuestion.ScaleThresholds, newQuestion.YesNoThresholds));
            }
            catch (ItemAlreadyExistsException iex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, iex.Message);
            }
        }
        [HttpPost, Route("UpdateSurvey")]
        public HttpResponseMessage UpdateSurvey(SurveyVM newSurvey)
        {

            var tagIds = !newSurvey.Tags.Any() ? new List<SurveyTag>() : newSurvey.Tags.Select(x => new SurveyTag() { Name = x.Name, Id = x.Id }).ToList();
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.UpdateSurvey(newSurvey.Id, newSurvey.Name, newSurvey.Description, tagIds, userGuid);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpPost, Route("UpdateQuestion")]
        public HttpResponseMessage UpdateQuestion(QuestionVM newQuestion)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.UpdateQuestion(newQuestion.QuestionId, newQuestion.Question, newQuestion.QuestionType, userGuid, newQuestion.ScaleThresholds, newQuestion.YesNoThresholds);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpGet, Route("GetQuestionDetails/{QuestionId:int}")]
        public b.Survey.QuestionDTO GetQuestionDetails(int QuestionId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _surveyMan.GetQuestionDetails(QuestionId, userGuid);

        }
        [HttpGet, Route("GetAllSurveyQuestions/{surveyId:int}")]
        public List<QuestionVM> GetAllSurveyQuestions(int surveyId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _surveyMan.GetAllSurveyQuestions(surveyId, userGuid).Select(x => new QuestionVM() {CanModify = x.CanModify, SurveysToQuestionsId = x.SurveysToQuestionsId, Question = x.Question, QuestionId = x.QuestionId, QuestionType = x.QuestionType, SurveyId = x.SurveyId }).ToList();
        }
        [HttpGet, Route("GetAllQuestions")]
        public List<QuestionVM> GetAllQuestions()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _surveyMan.GetAllQuestions(userGuid).Select(x => new QuestionVM() { CanModify = x.CanModify, Question = x.Question, QuestionId = x.QuestionId, QuestionType = x.QuestionType, SurveyId = x.SurveyId }).ToList();
        }
        [HttpPost, Route("AddQuestionToSurvey")]
        public int AddQuestionToSurvey(QuestionVM targetQuestion)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _surveyMan.AddQuestionToSurvey(targetQuestion.QuestionId, targetQuestion.SurveyId, userGuid);
        }
        [HttpPost, Route("RemoveQuestionFromSurvey/{SurveysToQuestionsId:int}")]
        public void RemoveQuestionFromSurvey(int SurveysToQuestionsId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.DeleteQuestionFromSurvey(SurveysToQuestionsId, userGuid);
        }
        [HttpPost, Route("AnswerYesNoQuestion")]
        public void AnswerYesNoQuestion([FromBody] Models.Athlete.CompletedQuestionYesNo answer)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.CompleteYesNoQuestion(answer, userGuid);
        }
        [HttpPost, Route("AnswerScaleQuestion")]
        public void AnswerScaleQuestion([FromBody] Models.Athlete.CompletedQuestionScale answer)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.CompleteScaleQuestion(answer, userGuid);
        }
        [HttpPost, Route("AnswerOpenEndedQuestion")]
        public void AnswerOpenEndedQuestion([FromBody] Models.Athlete.CompletedQuestionOpenEnded answer)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _surveyMan.CompleteOpenEndedQuestion(answer, userGuid);
        }
        [HttpGet, Route("GetHistoricProgramsWithSurveys/{athleteId:int}")]
        public List<HistoricProgram> GetAllHistoricProgramsWithSurveys(int athleteId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _surveyMan.GetAllHistoricProgramsWithSurveys(athleteId, userGuid);
        }
        [HttpGet, Route("GetAssignedProgramQuestions/{athleteId:int}/{assignedProgramId:int}")]
        public List<DAL.DTOs.AthleteAssignedPrograms.AthleteAssignedQuestions> GetAssignedProgramQuestions(int athleteId, int assignedProgramId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _programManager.GetAssignedQuestions(assignedProgramId, athleteId, userGuid);
        }

        [HttpGet, Route("GetSurveysByAssignedProgramId/{assignedProgramId:int}")]
        public List<DAL.DTOs.Survey.AthleteHomePageSurvey> GetHomePageSurveysByAssignedProgramId(int assignedProgramId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return _surveyMan.GetHomePageSurveysByAssignedProgramId(assignedProgramId, userGuid);
        }
    }
}
