using System;
using System.Collections.Generic;
using System.Linq;
using DAL.Repositories;
using Models.Survey;
using b = BL.BusinessObjects;
using Models.Enums;
using DAL.CustomerExceptions;
using BL.CustomExceptions;
using Models.Athlete;
using DAL.DTOs.AthleteAssignedPrograms;
using DAL.DTOs.Survey;

namespace BL
{
    public interface ISurveyManager
    {
        IAthleteRepo _athleteRepo { get; set; }

        int AddQuestionToSurvey(int questionId, int surveyId, Guid userToken);
        void AddScaleThreshold(b.Survey.NewScaleThreshold newScaleThreshold, Guid userToken);
        void AddTagsToSurvey(List<SurveyTag> tagIds, int surveyId, Guid createdUserGuid);
        void AddYesNoThreshold(b.Survey.NewYesNoThreshold newYesNoThreshold, Guid userToken);
        void ArchiveSurvey(int surveyId, Guid createdUserToken);
        void CompleteOpenEndedQuestion(CompletedQuestionOpenEnded answeredQuestion, Guid userToken);
        void CompleteScaleQuestion(CompletedQuestionScale answeredQuestion, Guid userToken);
        void CompleteYesNoQuestion(CompletedQuestionYesNo answeredQuestion, Guid userToken);
        int CreateNewSurvey(string title, string description, List<SurveyTag> TagIds, Guid createdUserGuid);
        int CreateQuestion(string questionText, QuestionTypeEnum qtype, int surveyId, Guid userToken, List<b.Survey.NewScaleThreshold> scaleQuestionThresholds, List<b.Survey.NewYesNoThreshold> yesNoQuestionThresholds);
        void DeleteQuestionFromSurvey(int SurveysToQuestionsId, Guid userToken);
        void DeleteScaleThreshold(b.Survey.NewScaleThreshold newScaleThreshold, Guid userToken, int targetThresholdId);
        void DeleteYesNoThreshold(b.Survey.NewYesNoThreshold newYesNoThreshold, Guid userToken, int targetThresholdId);
        void DuplicateSurvey(int surveyId, Guid createdUserToken);
        List<HistoricProgram> GetAllHistoricProgramsWithSurveys(int athleteId, Guid createdUserToken);
        List<b.Survey.QuestionDTO> GetAllQuestions(Guid userToken);
        List<b.Survey.QuestionDTO> GetAllSurveyQuestions(int surveyId, Guid userToken);
        List<b.Survey.Survey> GetAllSurveys(Guid userToken);
        List<AthleteHomePageSurvey> GetHomePageSurveysByAssignedProgramId(int assignedProgramId, Guid createdUserGuid);
        List<PastSurveyItem> GetPastSurveyList(int athleteId, Guid requestedUserToken);
        b.Survey.QuestionDTO GetQuestionDetails(int questionId, Guid userToken);
        void HardDelete(int surveyId, Guid userToken);
        void UnArchiveSurvey(int surveyId, Guid createdUserToken);
        void UpdateQuestion(int questionId, string questionText, QuestionTypeEnum qtype, Guid userToken, List<b.Survey.NewScaleThreshold> scaleQuestionThresholds, List<b.Survey.NewYesNoThreshold> yesNoQuestionThresholds);
        void UpdateScaleThreshold(b.Survey.NewScaleThreshold newScaleThreshold, Guid userToken, int targetThresholdId);
        void UpdateSurvey(int surveyId, string title, string description, List<SurveyTag> TagIds, Guid createdUserGuid);
        void UpdateYesNoThreshold(b.Survey.NewYesNoThreshold newYesNoThreshold, Guid userToken, int targetThresholdId);
    }

    public class SurveyManager : ISurveyManager
    {
        private ISurveyRepo _surveyRepo { get; set; }
        private ITagRepo<SurveyTag> _surveyTagRepo { get; set; }
        public IAthleteRepo _athleteRepo { get; set; }
        private List<OrganizationRoleEnum> _userRoles { get; set; }
        private IWeightRoomRepo _weightRoomRepo { get; set; }
        private IOrganizationRepo _orgRepo { get; set; }
        private IUserRepo _userRepo { get; set; }

        public SurveyManager(ISurveyRepo surveyRepo, IAthleteRepo athleteRepo, ITagRepo<SurveyTag> tagRepo, IWeightRoomRepo weightRoomRepo, IOrganizationRepo orgRepo, IUserRepo userRepo)
        {
            _surveyRepo = surveyRepo;
            _surveyTagRepo = tagRepo;
            _athleteRepo = athleteRepo;
            _orgRepo = orgRepo;
            _weightRoomRepo = weightRoomRepo;
            _userRepo = userRepo;
            _surveyTagRepo.InitializeTagRepo(TagEnum.Survey);
        }
        private void GenerateUserRoles(Guid userToken)
        {
            _userRoles = _orgRepo.GetUserRoles(userToken);
        }

        public void HardDelete(int surveyId, Guid userToken)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveSurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To DELETE Surveys");
            }
            var targetSurvey = _surveyRepo.GetSurvey(surveyId, userToken);
            if (!targetSurvey.CanModify) throw new ApplicationException("Cannot Delete This Survey As It Is In Use");

            _surveyTagRepo.DeleteAssociatedTags(surveyId);
            _surveyRepo.HardDelete(surveyId);

        }
        public List<PastSurveyItem> GetPastSurveyList(int athleteId, Guid requestedUserToken)
        {
            var targetUser = _athleteRepo.GetAthlete(athleteId);
            var targetCoach = _userRepo.Get(requestedUserToken);
            GenerateUserRoles(requestedUserToken);
            if (targetUser != null)
            {//athletes can view thier past surveys
                return _surveyRepo.GetPastSurveyList(athleteId);
            }
            if (targetUser.OrganizationId != targetCoach.OrganizationId)
            {
                throw new ApplicationException("User Does Not Have Rights To View Athlete Surveys");
            }
            if (!(_userRoles.Contains(OrganizationRoleEnum.ViewAthletes) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To View Athlete Surveys");
            }
            return _surveyRepo.GetPastSurveyList(athleteId);
        }
        public void UnArchiveSurvey(int surveyId, Guid createdUserToken)
        {
            GenerateUserRoles(createdUserToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveSurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To UnArchive Surveys");
            }
            _surveyRepo.UnArchiveSurvey(surveyId, createdUserToken);
        }
        public void ArchiveSurvey(int surveyId, Guid createdUserToken)
        {
            GenerateUserRoles(createdUserToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ArchiveSurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Archive Surveys");
            }
            _surveyRepo.ArchiveSurvey(surveyId, createdUserToken);
        }
        public void DuplicateSurvey(int surveyId, Guid createdUserToken)
        {
            GenerateUserRoles(createdUserToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateSurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Surveys");
            }
            _surveyRepo.DuplicateSurvey(surveyId, createdUserToken);
        }
        public List<HistoricProgram> GetAllHistoricProgramsWithSurveys(int athleteId, Guid createdUserToken)
        {
            return _surveyRepo.GetAllHistoricProgramsWithSurveys(athleteId, createdUserToken);
        }
        public void UpdateSurvey(int surveyId, string title, string description, List<SurveyTag> TagIds, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ModifySurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Survey");
            }
            try
            {
                if (!_userRepo.Get(createdUserGuid).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            _surveyRepo.UpdateSurvey(new Models.Survey.Survey() { Id = surveyId, Description = description, Title = title }, createdUserGuid);
            AddTagsToSurvey(TagIds, surveyId, createdUserGuid);
            return;
        }
        public void UpdateQuestion(int questionId, string questionText, QuestionTypeEnum qtype, Guid userToken, List<b.Survey.NewScaleThreshold> scaleQuestionThresholds, List<b.Survey.NewYesNoThreshold> yesNoQuestionThresholds)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ModifySurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Survey");
            }
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            _surveyRepo.UpdateQuestion(questionText, qtype, questionId, userToken);
            //remove all old notifications
            //then re add
            //easier then updating a crap ton of notifications
            _surveyRepo.RemoveCoachesFromScaleThresholdNotification(questionId);
            _surveyRepo.RemoveCoachesFromYesNoThresholdNotification(questionId);

            _surveyRepo.RemoveScaleThreshold(questionId);
            _surveyRepo.RemoveYesNoThreshold(questionId);


            if (scaleQuestionThresholds != null)//in reality they will only send up either a scale or a yesno. doing this incase somehow in the future the want to send up both
            {
                scaleQuestionThresholds.ForEach(x =>
                {
                    x.QuestionId = questionId;
                    AddScaleThreshold(x, userToken);
                });
            }
            if (yesNoQuestionThresholds != null)//in reality they will only send up either a scale or a yesno. doing this incase somehow in the future the want to send up both
            {
                yesNoQuestionThresholds.ForEach(x =>
                {
                    x.QuestionId = questionId;
                    AddYesNoThreshold(x, userToken);
                });
            }
        }
        public int CreateNewSurvey(string title, string description, List<SurveyTag> TagIds, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateSurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Survey");
            }
            try
            {
                if (!_userRepo.Get(createdUserGuid).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            if (string.IsNullOrEmpty(title))
            {
                throw new ItemValidationError("Survey Name Is Invalid. Please Change the name");
            }
            var newId = 0;
            try
            {
                newId = _surveyRepo.CreateSurvey(new Models.Survey.Survey() { Description = description, Title = title }, createdUserGuid);
            }
            catch (DuplicateKeyException dup)
            {
                throw new ItemAlreadyExistsException("This Survey With This Name Has Already Exists", dup);

            }
            AddTagsToSurvey(TagIds, newId, createdUserGuid);
            return newId;
        }
        public void AddTagsToSurvey(List<SurveyTag> tagIds, int surveyId, Guid createdUserGuid)
        {
            GenerateUserRoles(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateMetrics) || _userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifySurveys)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Surveys");
            }
            try
            {
                if (!_userRepo.Get(createdUserGuid).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            var targetSurvey = _surveyRepo.GetSurvey(surveyId, createdUserGuid);
            if (targetSurvey == null) return;

            _surveyTagRepo.DeleteAssociatedTags(surveyId);
            _surveyTagRepo.AddAssociatedTags(tagIds, surveyId);
        }
        public List<b.Survey.Survey> GetAllSurveys(Guid userToken)
        {
            var allMapings = _surveyRepo.GetAllSurveyTagMappings(userToken);

            var allSurveys = _surveyRepo.GetAllSurveys(userToken);

            var ret = new List<b.Survey.Survey>();

            foreach (var s in allSurveys)
            {
                var mapping = allMapings.FirstOrDefault(x => x.SurveyId == s.Id);

                ret.Add(new b.Survey.Survey
                {
                    Id = s.Id,
                    Description = s.Description,
                    Name = s.Title,
                    CanModify = s.CanModify,
                    IsDeleted = s.IsDeleted,
                    Tags = mapping == null ? new List<b.Tag>() : mapping.Tags.Select(x => new b.Tag() { Id = x.Id, Name = x.Name }).ToList()
                });
            }
            return ret;
        }
        public int CreateQuestion(string questionText, QuestionTypeEnum qtype, int surveyId, Guid userToken, List<b.Survey.NewScaleThreshold> scaleQuestionThresholds, List<b.Survey.NewYesNoThreshold> yesNoQuestionThresholds)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateSurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Surveys");
            }
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }

            var questionId = 0;
            try
            {
                questionId = _surveyRepo.CreateQuestion(questionText, qtype, userToken);
            }
            catch (DuplicateKeyException dup)
            {
                throw new ItemAlreadyExistsException("A Question With This Name Already Exists", dup);
            }

            if (surveyId != 0)
            {
                var targetSurvey = _surveyRepo.GetSurvey(surveyId, userToken);
                _surveyRepo.MapQuestionToSurvey(surveyId, questionId);
            }
            if (scaleQuestionThresholds != null)//in reality they will only send up either a scale or a yesno. doing this incase somehow in the future the want to send up both
            {
                scaleQuestionThresholds.ForEach(x =>
                {
                    x.QuestionId = questionId;
                    AddScaleThreshold(x, userToken);
                });
            }
            if (yesNoQuestionThresholds != null)//in reality they will only send up either a scale or a yesno. doing this incase somehow in the future the want to send up both
            {
                yesNoQuestionThresholds.ForEach(x =>
                {
                    x.QuestionId = questionId;
                    AddYesNoThreshold(x, userToken);
                });
            }
            return questionId;
        }
        public int AddQuestionToSurvey(int questionId, int surveyId, Guid userToken)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateSurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifySurveys)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Surveys");
            }
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            var targetSurvey = _surveyRepo.GetSurvey(surveyId, userToken);
            if (targetSurvey == null) return 0;

            return _surveyRepo.AssociateQuestionWithSurvey(questionId, surveyId);
        }
        public void DeleteQuestionFromSurvey(int SurveysToQuestionsId, Guid userToken)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.ModifySurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Surveys");
            }
            try
            {
                if (!_userRepo.Get(userToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Exercises");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            var targetSurveyId = _surveyRepo.GetSurveyIdOfMappedQuestion(SurveysToQuestionsId);
            var survey = _surveyRepo.GetSurvey(targetSurveyId, userToken);
            if (survey == null) return;

            _surveyRepo.RemoveQuestionFromSurvey(SurveysToQuestionsId);
        }
        public List<b.Survey.QuestionDTO> GetAllQuestions(Guid userToken)
        {
            return _surveyRepo.GetAllQuestions(userToken).Select(x => new b.Survey.QuestionDTO() { CanModify = x.CanModify, QuestionId = x.Id, Question = x.QuestionDisplayText, QuestionType = (QuestionTypeEnum)x.QuestionTypeId, SurveyId = x.SurveyId }).ToList();
        }
        public List<b.Survey.QuestionDTO> GetAllSurveyQuestions(int surveyId, Guid userToken)
        {
            return _surveyRepo.GetAllSurveyQuestions(surveyId, userToken).Select(x => new b.Survey.QuestionDTO() { CanModify = x.CanModify, SurveysToQuestionsId = x.SurveysToQuestionsId, QuestionId = x.Id, Question = x.QuestionDisplayText, QuestionType = (QuestionTypeEnum)x.QuestionTypeId, SurveyId = x.SurveyId }).ToList();
        }
        public void CompleteYesNoQuestion(CompletedQuestionYesNo answeredQuestion, Guid userToken)
        {
            var validatedId = _athleteRepo.ValidateAthleteToken(userToken, answeredQuestion.AthleteId);//this will validate the token to make sure its the target athlete
            GenerateUserRoles(userToken);                                                                    //so another athlete doesnt update an different athletes shit. 

            var targetUser = _userRepo.Get(userToken); //getting the user by the token becuase it could be a coach trying to update
            var userRoles = _orgRepo.GetUserRoles(userToken);
            var weightRoomAccount = _weightRoomRepo.Get(userToken);
            if (weightRoomAccount == null && !validatedId && !targetUser.IsCoach) throw new Exception("Wrong Athlete infromation");

            var targetAthlete = _athleteRepo.GetAthlete(answeredQuestion.AthleteId);

            if (targetAthlete == null && weightRoomAccount == null && !targetUser.IsCoach && !(userRoles.Contains(OrganizationRoleEnum.Admin) || userRoles.Contains(OrganizationRoleEnum.ModifyAthletes)))
            {
                throw new Exception("This User Cannot Modify This Athletes Data");
            }

            answeredQuestion.CompletedDate = DateTime.Now;
            answeredQuestion.AthleteId = answeredQuestion.AthleteId;
            
            if (targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
            {
                answeredQuestion.AssignedProgramId = targetAthlete.AssignedProgram_AssignedProgramId.Value;
                _surveyRepo.AssignedProgram_CompleteYesNoQuestion(answeredQuestion);
            }
            else {
                answeredQuestion.AssignedProgramId = targetAthlete.AssignedProgramId.Value;
                _surveyRepo.CompleteYesNoQuestion(answeredQuestion);
            }
            
            _surveyRepo.CompleteYesNoQuestion(answeredQuestion);
            _surveyRepo.SendOutNotificationsForYesNow(answeredQuestion.QuestionId, answeredQuestion.AthleteId, answeredQuestion.YesNoValue);
        }
        public void CompleteScaleQuestion(CompletedQuestionScale answeredQuestion, Guid userToken)
        {
            var validatedId = _athleteRepo.ValidateAthleteToken(userToken, answeredQuestion.AthleteId);//this will validate the token to make sure its the target athlete
                                                                                                       //so another athlete doesnt update an different athletes shit. 

            var targetUser = _userRepo.Get(userToken); //getting the user by the token becuase it could be a coach trying to update
            var userRoles = _orgRepo.GetUserRoles(userToken);
            GenerateUserRoles(userToken);
            var weightRoomAccount = _weightRoomRepo.Get(userToken);
            if (weightRoomAccount == null && !validatedId && !targetUser.IsCoach) throw new Exception("Wrong Athlete infromation");

            var targetAthlete = _athleteRepo.GetAthlete(answeredQuestion.AthleteId);//at this point it is either the athlete or a coach with the ability to modify athelets. Thats why I am trusting the user data

            if (targetAthlete == null && weightRoomAccount == null && !targetUser.IsCoach && !(userRoles.Contains(OrganizationRoleEnum.Admin) || userRoles.Contains(OrganizationRoleEnum.ModifyAthletes))) throw new Exception("This User Cannot Modify This Athletes Data");

            answeredQuestion.CompletedDate = DateTime.Now;
            answeredQuestion.AthleteId = answeredQuestion.AthleteId;
            

            if (targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
            {
                answeredQuestion.AssignedProgramId = targetAthlete.AssignedProgram_AssignedProgramId.Value;
                _surveyRepo.AssignedProgram_CompleteScaleQuestion(answeredQuestion);
            }
            else
            {
                answeredQuestion.AssignedProgramId = targetAthlete.AssignedProgramId.Value;
                _surveyRepo.CompleteScaleQuestion(answeredQuestion);
            }
            
            _surveyRepo.SendOutNotificationsForScaleQuestion(answeredQuestion.QuestionId, answeredQuestion.AthleteId, answeredQuestion.ScaleValue);
        }
        public void CompleteOpenEndedQuestion(CompletedQuestionOpenEnded answeredQuestion, Guid userToken)
        {
            var validatedId = _athleteRepo.ValidateAthleteToken(userToken, answeredQuestion.AthleteId);//this will validate the token to make sure its the target athlete
            GenerateUserRoles(userToken);                                                           //so another athlete doesnt update an different athletes shit. 

            var targetUser = _userRepo.Get(userToken); //getting the user by the token becuase it could be a coach trying to update
            var userRoles = _orgRepo.GetUserRoles(userToken);
            var weightRoomAccount = _weightRoomRepo.Get(userToken);
            if (weightRoomAccount == null && !validatedId && !targetUser.IsCoach) throw new Exception("Wrong Athlete infromation");

            var targetAthlete = _athleteRepo.GetAthlete(answeredQuestion.AthleteId);//at this point it is either the athlete or a coach with the ability to modify athelets. Thats why I am trusting the user data

            if (targetAthlete == null && weightRoomAccount == null && !targetUser.IsCoach && !(userRoles.Contains(OrganizationRoleEnum.Admin) || userRoles.Contains(OrganizationRoleEnum.ModifyAthletes))) throw new Exception("This User Cannot Modify This Athletes Data");

            answeredQuestion.CompletedDate = DateTime.Now;
            answeredQuestion.AthleteId = answeredQuestion.AthleteId;
            

            if (targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
            {
                answeredQuestion.AssignedProgramId = targetAthlete.AssignedProgram_AssignedProgramId.Value;
                _surveyRepo.AssignedProgram_CompleteOpenEndedQuestion(answeredQuestion);
            }
            else {
                answeredQuestion.AssignedProgramId = targetAthlete.AssignedProgramId.Value;
                _surveyRepo.CompleteOpenEndedQuestion(answeredQuestion);
            }
        }
        public List<DAL.DTOs.Survey.AthleteHomePageSurvey> GetHomePageSurveysByAssignedProgramId(int assignedProgramId, Guid createdUserGuid)
        {
            return _surveyRepo.GetHomePageSurveysByAssignedProgramId(assignedProgramId, createdUserGuid);
        }
        public void AddScaleThreshold(b.Survey.NewScaleThreshold newScaleThreshold, Guid userToken)
        {
            ExecuteBuisnessLogicChecksForAddingThresholds(newScaleThreshold.QuestionId, userToken, newScaleThreshold.CoachIds);
            var newThreshold = _surveyRepo.AddScaleThreshold(newScaleThreshold.QuestionId, newScaleThreshold.Comparer, newScaleThreshold.ThresholdValue);
            _surveyRepo.AddCoachEmailToScaleThresholdNotification(newThreshold, newScaleThreshold.CoachIds);
        }
        public void AddYesNoThreshold(b.Survey.NewYesNoThreshold newYesNoThreshold, Guid userToken)
        {
            ExecuteBuisnessLogicChecksForAddingThresholds(newYesNoThreshold.QuestionId, userToken, newYesNoThreshold.CoachIds);

            var newThreshold = _surveyRepo.AddYesNoThreshold(newYesNoThreshold.QuestionId, newYesNoThreshold.ThresholdValue);
            _surveyRepo.AddCoachEmailToYesNoThresholdNotification(newThreshold, newYesNoThreshold.CoachIds);
        }
        public void UpdateScaleThreshold(b.Survey.NewScaleThreshold newScaleThreshold, Guid userToken, int targetThresholdId)
        {
            throw new NotImplementedException("Removed Because Updateing Thresholds can only be done when updating a question ");
        }
        public void UpdateYesNoThreshold(b.Survey.NewYesNoThreshold newYesNoThreshold, Guid userToken, int targetThresholdId)
        {
            throw new NotImplementedException("Removed Because Updateing Thresholds can only be done when updating a question ");
        }
        public void DeleteYesNoThreshold(b.Survey.NewYesNoThreshold newYesNoThreshold, Guid userToken, int targetThresholdId)
        {
            throw new NotImplementedException("Removed because you can only update thresholds when updating a question");
        }
        public void DeleteScaleThreshold(b.Survey.NewScaleThreshold newScaleThreshold, Guid userToken, int targetThresholdId)
        {
            throw new NotImplementedException("Removed because you can only update thresholds when updating a question");
        }

        public b.Survey.QuestionDTO GetQuestionDetails(int questionId, Guid userToken)
        {
            var user = _userRepo.Get(userToken);
            var question = _surveyRepo.GetQuestion(questionId, userToken);
            if (question == null) throw new ApplicationException("You Can Only Access Questions Created By Your Organization");

            var ret = new b.Survey.QuestionDTO()
            {
                QuestionId = question.Id,
                Question = question.QuestionDisplayText,
                QuestionType = (QuestionTypeEnum)question.QuestionTypeId,
                CanModify = question.CanModify

            };

            var yesNoThresholds = _surveyRepo.GetYesNoThresholdsForQuestion(questionId);
            var scaleThresholds = _surveyRepo.GetScaleThresholdsForQuestion(questionId);

            if (yesNoThresholds.Any())
            {
                ret.YesNoThresholds = new List<b.Survey.NewYesNoThreshold>();

                yesNoThresholds.ForEach(x =>
                {
                    var coachIds = _surveyRepo.GetThresholdNotificationsForCoachesYesNoThreshold(x.Id);
                    ret.YesNoThresholds.Add(new b.Survey.NewYesNoThreshold()
                    {
                        QuestionId = question.Id,
                        ThresholdValue = x.Threshold,
                        CoachIds = coachIds
                    });
                });

            }
            else if (scaleThresholds.Any())
            {
                ret.ScaleThresholds = new List<b.Survey.NewScaleThreshold>();

                scaleThresholds.ForEach(x =>
                {
                    var coachIds = _surveyRepo.GetThresholdNotificationsForCoachesScaleThreshold(x.Id);
                    ret.ScaleThresholds.Add(new b.Survey.NewScaleThreshold()
                    {
                        CoachIds = coachIds,
                        Comparer = x.Comparer,
                        QuestionId = question.Id,
                        ThresholdValue = x.ThresholdValue
                    });
                });
            }
            return ret;
        }
        private void ExecuteBuisnessLogicChecksForAddingThresholds(int questionId, Guid userToken, List<int> coachIds)
        {
            if (!(_userRoles.Contains(OrganizationRoleEnum.ModifySurveys) || _userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.CreateSurveys)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify/Create Surveys");
            }
            var targetUser = _userRepo.Get(userToken);
            var targetQuestion = _surveyRepo.GetQuestion(questionId, userToken);

            if (targetQuestion == null)
            {
                throw new ApplicationException("The Question You Are Modifying Doesnt Belong To Your Organization");
            }
            foreach (var coach in coachIds)
            {
                var targetCoach = _userRepo.Get(userToken);
                if (targetCoach.OrganizationId != targetUser.OrganizationId)
                {
                    throw new ApplicationException("You Are Attempting To Add A Coach To A Notification That Doesnt Belong To Your Organization");
                }
            }
        }
    }
}
