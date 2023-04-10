using AzureFunctions;
using CryptSharp;
using DAL.DTOs;
using DAL.DTOs.Metrics;
using DAL.DTOs.Program;
using DAL.Repositories;
using Models.Athlete;
using Models.Enums;
using Models.Metric;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net.Http;

namespace BL
{
    public interface IAthleteManager
    {
        void AddCompletedSet(CompletedSet targetSet, Guid userToken);
        void AddCompletedSuperSet(CompletedSuperSet_Set targetSet, Guid userToken);
        int AddCompleteMetric(CompletedMetric targetMetric, Guid userToken);
        void AddHeightWeight(int athleteId, Guid coachId, double? heightPrimary, double? heightSecondary, double? weight, Guid _userToken);
        void AddTagsToAthlete(List<AthleteTag> tagIds, int athleteId, Guid createdUserGuid);
        void ArchiveAthlete(int athleteId, Guid coachToken);
        void FinishAthleteRegistration(string userName, string password, int athleteId, string emailValidationToken);
        void fixDuplicateAccounts();
        DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAnAthletesAssignedProgram(Guid userToken, int athleteId, int assignedProgramId = 0 , bool isSnapShot = false);
        DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgram(Guid userToken, int assignedProgramID = 0);
        DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAthleteByWeightRoom(int athleteId, Guid weightRoomTokenId);
        List<CompletedMetricDisplay> GetAthleteListOfCompletedMetrics(int athleteId, Guid _userToken);
        List<ProgramHistory> GetAthleteProgramHistory(int athleteId, Guid _userToken);
        List<CompletedMetricHistory> GetMetricHistory(int metricId, int athleteId, Guid _userToken);
        void MarkDayCompleted(int programDayId, int assignedProgramId, Guid userToken);
        void MarkWeekCompleted(int weekId, int assignedProgramId, Guid userToken);
        void PrintWorkout(Guid userToken, int programId, int athleteId);
        void UnassignProgramToAthlete(int athleteId, Guid createdUserToken);
        void UpdateAthlete(AthleteDTO newAthlete, List<AthleteTag> tagIds, List<AddedMetric> addedMetrics, Guid userToken);
        void UpdateMetric(int id, double value, DateTime completedDate, bool IsCompletedMetric, Guid _userToken);
    }

    public class AthleteManager : IAthleteManager
    {
        private static string AzureFunctionsBaseURL => ConfigurationManager.AppSettings["AzureFunctionsBaseURL"];
        private IAthleteRepo _athleteRepo { get; set; }
        private IProgramRepo _programRepo { get; set; }
        private List<OrganizationRoleEnum> _userRoles { get; set; }
        private ITagRepo<AthleteTag> _athleteTagRepo { get; set; }
        private IWeightRoomRepo _weightRoomRepo { get; set; }
        private IOrganizationRepo _orgRepo { get; set; }
        private IUserRepo _userRepo { get; set; }

        public AthleteManager(IAthleteRepo athleteRepo, IProgramRepo programRepo, ITagRepo<AthleteTag> athleteTagRepo, IWeightRoomRepo weightRoomRepo, IOrganizationRepo orgRepo, IUserRepo userRepo)
        {
            _athleteRepo = athleteRepo;
            _programRepo = programRepo;
            _weightRoomRepo = weightRoomRepo;
            _athleteTagRepo = athleteTagRepo;
            _athleteTagRepo.InitializeTagRepo(TagEnum.Athlete);
            _orgRepo = orgRepo;
            _userRepo = userRepo;

        }
        private void GenerateUserRoles(Guid userToken)
        {
            _userRoles = _orgRepo.GetUserRoles(userToken);
        }
        public void UnassignProgramToAthlete(int athleteId, Guid createdUserToken)
        {
            _athleteRepo.UnassignProgramToAthlete(athleteId, createdUserToken);
        }


        public DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAthleteByWeightRoom(int athleteId, Guid weightRoomTokenId)
        {
            var athlete = _athleteRepo.GetAthleteByWeightRoom(athleteId, weightRoomTokenId);
            return _programRepo.GetAssignedProgram(athlete);
        }


        public List<ProgramHistory> GetAthleteProgramHistory(int athleteId, Guid _userToken)
        {
            var targetAthlete = _athleteRepo.GetAthlete(_userToken);
            GenerateUserRoles(_userToken);
            if (targetAthlete != null)
            {
                return _athleteRepo.GetAthleteProgramHistory(athleteId);
            }
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ViewAthletes)))
            {
                throw new ApplicationException("User Does Not Have Rights To View Athletes");
            }
            return _athleteRepo.GetAthleteProgramHistory(athleteId);
        }
        public List<CompletedMetricHistory> GetMetricHistory(int metricId, int athleteId, Guid _userToken)
        {
            var targetAthlete = _athleteRepo.GetAthlete(_userToken);
            GenerateUserRoles(_userToken);
            if (targetAthlete != null)
            {
                return _athleteRepo.GetMetricHistory(metricId, athleteId);
            }
            else if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ViewAthletes)))
            {
                throw new ApplicationException("User Does Not Have Rights To View Athletes");
            }
            var X= _athleteRepo.GetMetricHistory(metricId, athleteId);
            return _athleteRepo.GetMetricHistory(metricId, athleteId);
        }
        /// <summary>
        /// Fixes Duiplicate accounts with Respect to the user's email
        /// </summary>
        public void fixDuplicateAccounts()
        {
            //_athleteRepo.fixDuplicateAccounts(); I DONT EVEN KNOW IF THIS WORKS 
        }
        public void UpdateMetric(int id, double value, DateTime completedDate, bool IsCompletedMetric, Guid _userToken)
        {
            var targetAthlete = _athleteRepo.GetAthlete(_userToken);
            GenerateUserRoles(_userToken);
            if (targetAthlete != null)
            {
                _athleteRepo.UpdateMetric(id, IsCompletedMetric, value, completedDate);
            }
            else if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifyAthletes)))
            {
                throw new ApplicationException("User Does Not Have Rights To View Athletes");
            }

            _athleteRepo.UpdateMetric(id, IsCompletedMetric, value, completedDate);

        }

        public List<CompletedMetricDisplay> GetAthleteListOfCompletedMetrics(int athleteId, Guid _userToken)
        {
            var targetAthlete = _athleteRepo.GetAthlete(_userToken);
            GenerateUserRoles(_userToken);
            if (targetAthlete != null)
            {
                return _athleteRepo.GetAthleteListOfCompletedMetrics(athleteId);
            }
            else if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ViewAthletes)))
            {
                throw new ApplicationException("User Does Not Have Rights To View Athletes");
            }
            return _athleteRepo.GetAthleteListOfCompletedMetrics(athleteId);
        }

        public void PrintWorkout(Guid userToken, int programId, int athleteId)
        {
            var athlete = _athleteRepo.GetAthlete(userToken);
            GenerateUserRoles(userToken);
            if (athlete == null)//must be a coach trying
            {
                var headCoach = _userRepo.Get(userToken);

                if (!_userRoles.Contains(OrganizationRoleEnum.PrintPrograms) && !_userRoles.Contains(OrganizationRoleEnum.Admin)) throw new ApplicationException("You Cannot Print This Program");
                athlete = _athleteRepo.GetAthlete(athleteId);
            }

            else if (athlete.Id != athleteId) //must be an athlete make sure its the same one and not someone else
            {
                throw new ApplicationException("You Cannot Print This Program");
            }

            //if they made it through its either a coach with print privlages or the correct athlete
            var assignedWorkout = _programRepo.GetAssignedProgram(athlete);//programRepo.GetAssignedProgram(targetAthlete);

            var DoesProgramUseAdvancedFeature = false;

            assignedWorkout.Days.ForEach(x =>
            {
                x.AssignedSuperSets.ForEach(y =>
                {
                    y.Exercises.ForEach(z =>
                    {
                        z.SetsAndReps.ForEach(a =>
                        {
                            if (!DoesProgramUseAdvancedFeature)
                            {
                                DoesProgramUseAdvancedFeature = a.RepsAchieved
                                                                    || a.AssignedWorkoutMinutes.HasValue
                                                                    || !string.IsNullOrEmpty(a.AssignedWorkoutDistance)
                                                                    || a.AssignedWorkoutSeconds.HasValue;

                            }
                        });
                    });
                });
            });

            if (DoesProgramUseAdvancedFeature)
            {
                //todo: george we will need to change the code
                var res = new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/AthleteNewPdf?code={Config.GenerateNewPdfCode}",
                 new StringContent(JsonConvert.SerializeObject(new PrintPDFOptionsAthleteOnly() { CreatedUserToken = userToken, ProgramId = programId, AthleteId = athleteId })));
            }
            else
            {
                var res = new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/GenerateAthletePDF?code={Config.GeneratePDFCode}",
                 new StringContent(JsonConvert.SerializeObject(new PrintPDFOptionsAthleteOnly() { CreatedUserToken = userToken, ProgramId = programId, AthleteId = athleteId })));
            }
        }
        public void UpdateAthlete(DAL.DTOs.AthleteDTO newAthlete, List<AthleteTag> tagIds, List<AddedMetric> addedMetrics, Guid userToken)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifyAthletes)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Athletes");
            }
            var createdUser = _userRepo.Get(userToken);
            if (!createdUser.IsCoach)
                throw new ApplicationException("Only Coachs Can Update Athletes.");
            var targetAthlete = _athleteRepo.GetAthlete(newAthlete.Id);
            var existingEmailCheck = _userRepo.GetByEmail(newAthlete.Email);

            if (existingEmailCheck != null && existingEmailCheck.Id != targetAthlete.AthleteUserId)
            {
                throw new ApplicationException("There is already a User account with this email, please choose another email.");
            }

            _athleteRepo.UpdateAthlete(newAthlete, userToken);
            _userRepo.UpdateEmail(newAthlete.Email, targetAthlete.AthleteUserId);

            if (newAthlete.Weight.HasValue || newAthlete.HeightPrimary.HasValue || newAthlete.HeightSecondary.HasValue)
            {
                _athleteRepo.UpdateAthleteHeightWeight(newAthlete.Id, newAthlete.HeightPrimary, newAthlete.HeightSecondary, newAthlete.Weight);
            }

            AddTagsToAthlete(tagIds, newAthlete.Id, userToken);

            addedMetrics.ForEach(x => _athleteRepo.AddStandAloneMetric(newAthlete.Id, x.MetricId, x.Value, createdUser.Id, x.CompletedDate));
        }
        public void AddHeightWeight(int athleteId, Guid coachId, double? heightPrimary, double? heightSecondary, double? weight, Guid _userToken)
        {
            GenerateUserRoles(_userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifyAthletes)))
            {
                _athleteRepo.UpdateAthleteHeightWeight(athleteId, heightPrimary, heightSecondary, weight);
            }
        }
        public void ArchiveAthlete(int athleteId, Guid coachToken)
        {
            var athlete = _athleteRepo.GetAthlete(athleteId);
            var user = _userRepo.Get(coachToken);
            GenerateUserRoles(coachToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ArchiveAthletes)) && athlete.OrganizationId != user.OrganizationId)
            {
                throw new ApplicationException("User Cannot Archive Athletes");
            }
            _athleteRepo.Archive(athleteId, coachToken);
            //    DowngradeCoachSubscriptionIfApplicable(user);
        }

        public void FinishAthleteRegistration(string userName, string password, int athleteId, string emailValidationToken)
        {
            var salt = Guid.NewGuid();//todo: i dont know why we need this. further investigate please

            var user = _userRepo.Get(userName);
            if (!string.IsNullOrEmpty(user.UserName))
                throw new ApplicationException("User Name Is Already Taken");

            var targetAthlete = _athleteRepo.GetAthlete(athleteId);
            if (targetAthlete.ValidatedEmail)
                throw new ApplicationException("This User Has Already Been Registered");

            _athleteRepo.FinishAthleteCreation(userName, Crypter.Blowfish.Crypt(password), athleteId, emailValidationToken);
        }
        public DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAnAthletesAssignedProgram(Guid userToken, int athleteId, int assignedProgramId = 0, bool isSnapShot = false)
        {
            var wieghtRoomUser = _weightRoomRepo.Get(userToken);
            var targetAthlete = _athleteRepo.GetAthlete(athleteId, userToken);
            if (wieghtRoomUser != null)
            {
                targetAthlete = _athleteRepo.GetAthlete(athleteId);
            }
            if (assignedProgramId == 0)
            {
                if (targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
                {
                    var ret = _programRepo.GetAssignedProgramSnapShot(targetAthlete.AssignedProgram_AssignedProgramId.Value, userToken, targetAthlete.Id);
                    ret.IsSnapShot = true;
                    return ret;
                }
                var r = _programRepo.GetAssignedProgram(targetAthlete, targetAthlete.AssignedProgramId.Value);
                r.IsSnapShot = false;
                return r;

            }
            if (isSnapShot)
            {
                var ret = _programRepo.GetAssignedProgramSnapShot(assignedProgramId, userToken, targetAthlete.Id);
            }
            var re = _programRepo.GetAssignedProgram(targetAthlete, assignedProgramId);
            re.IsSnapShot = false;
            return re;
        }
        public DAL.DTOs.AthleteAssignedPrograms.AssignedProgram GetAssignedProgram(Guid userToken, int assignedProgramID = 0)
        {
            var targetAthlete = _athleteRepo.GetAthlete(userToken);
            if (targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
            {
                var ret = _programRepo.GetAssignedProgramSnapShot(targetAthlete.AssignedProgram_AssignedProgramId.Value, userToken, targetAthlete.Id);
                
                ret.IsSnapShot = true;
                return ret;
            }
            var r = _programRepo.GetAssignedProgram(targetAthlete, targetAthlete.AssignedProgramId.Value);
            r.IsSnapShot = false;
            return r;


        }
        public int AddCompleteMetric(Models.Athlete.CompletedMetric targetMetric, Guid userToken)
        {
            var validatedId = _athleteRepo.ValidateAthleteToken(userToken, targetMetric.AthleteId);//this will validate the token to make sure its the target athlete
                                                                                                   //so another athlete doesnt update an different athletes shit. 

            var targetUser = _userRepo.Get(userToken); //getting the user by the token becuase it could be a coach trying to update
            var weightRoomAccount = _weightRoomRepo.Get(userToken);
            if (!validatedId && !targetUser.IsCoach && weightRoomAccount == null)
                throw new Exception("Wrong Athlete infromation");

            if (!validatedId && weightRoomAccount == null && !targetUser.IsCoach && !(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifyAthletes)))
                throw new Exception("This User Cannot Modify This Athletes Data");



            var targetAthlete = _athleteRepo.GetAthlete(targetMetric.AthleteId);

            if (targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
            {
                _athleteRepo.AddCompletedMetricAssignedProgram_MetricsDisplayWeek(targetMetric);
                return targetMetric.ProgramDayItemMetricId;
            }
            else
            {
                return _athleteRepo.AddCompletedMetric(targetMetric);
            }

        }
        public void AddCompletedSet(Models.Athlete.CompletedSet targetSet, Guid userToken)
        {
            var validatedId = _athleteRepo.ValidateAthleteToken(userToken, targetSet.AthleteId);
            var weightRoomAccount = _weightRoomRepo.Get(userToken);
            if (weightRoomAccount == null && !validatedId)
                throw new Exception("Wrong login infromation");

            _athleteRepo.AddCompletedSet(targetSet);
        }
        public void AddCompletedSuperSet(Models.Athlete.CompletedSuperSet_Set targetSet, Guid userToken)
        {
            var validatedId = _athleteRepo.ValidateAthleteToken(userToken, targetSet.AthleteId);//this will validate the token to make sure its the target athlete
                                                                                                //so another athlete doesnt update an different athletes shit. 

            var targetUser = _userRepo.Get(userToken); //getting the user by the token becuase it could be a coach trying to update
            var weightRoomAccount = _weightRoomRepo.Get(userToken);
            if (!validatedId && !targetUser.IsCoach && weightRoomAccount == null)
                throw new Exception("Wrong Athlete infromation");

            if (!validatedId && weightRoomAccount == null && !targetUser.IsCoach && !(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.ModifyAthletes)))
                throw new Exception("This User Cannot Modify This Athletes Data");


            var targetAthlete = _athleteRepo.GetAthlete(targetSet.AthleteId);

            if (targetAthlete.AssignedProgram_AssignedProgramId.HasValue)
            {
                _athleteRepo.AddCompletedAssignedProgram_ProgramDayItemSuperSet_Set(targetSet);
            }
            else
            {
                _athleteRepo.AddCompletedSuperSet(targetSet);
            }
        }

        public void MarkDayCompleted(int programDayId, int assignedProgramId, Guid userToken)
        {
            var athlete = _athleteRepo.GetAthlete(userToken);
            var weightRoomAccount = _weightRoomRepo.Get(userToken);
            if (weightRoomAccount == null && athlete.AssignedProgramId != assignedProgramId)
                throw new Exception("You cannot modify this program");

            _athleteRepo.MarkDayCompleted(programDayId, assignedProgramId, athlete);
        }
        public void MarkWeekCompleted(int weekId, int assignedProgramId, Guid userToken)
        {
            var athlete = _athleteRepo.GetAthlete(userToken);
            if (athlete.AssignedProgramId != assignedProgramId)
                throw new Exception("You cannot modify this program");

            _athleteRepo.MarkWeekCompleted(weekId, assignedProgramId, athlete);
        }

        public void AddTagsToAthlete(List<AthleteTag> tagIds, int athleteId, Guid createdUserGuid)
        {
            var user = _userRepo.Get(createdUserGuid);
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.CreateAthletes) || _userRoles.Contains(OrganizationRoleEnum.ModifyAthletes)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Athletes");
            }
            var targetAthlete = _athleteRepo.GetAthlete(athleteId, createdUserGuid);
            if (targetAthlete == null)
                return;


            _athleteTagRepo.DeleteAssociatedTags(athleteId);
            _athleteTagRepo.AddAssociatedTags(tagIds, athleteId);
        }
    }
}
