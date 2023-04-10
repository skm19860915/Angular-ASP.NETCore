using DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using Models.Athlete;
using b = BL.BusinessObjects;
using CryptSharp;
using DAL.DTOs;
using System.Net.Http;
using Newtonsoft.Json;
using System.Text;
using AzureFunctions;
using Models.Enums;
using Models.Metric;
using Models.User;
using System.Configuration;
using Stripe;
using Stripe.Checkout;
using DAL.DTOs.Athlete;
using System.Threading.Tasks;
using System.Threading;

namespace BL
{
    public interface IRosterManager
    {
        void AddProfilePictureToAthlete(int athleteId, int picId, Guid userToken);
        void AddTagsToAthlete(List<AthleteTag> tagIds, int athleteId, Guid createdUserGuid);
        Task AssignProgramToAthletes(List<int> athleteIds, int programId, Guid UserToken, DateTime startDate);
        List<AssignedProgramAthleteDTO> CheckAthletesAssignedPrograms(List<int> AthleteIds, Guid UserToken);
        Task CreateAssistantCoach(string firstName, string lastName, string email, Guid userToken);
        int CreateAthlete(AthleteDTO newAthlete, List<AthleteTag> tagIds, Guid userToken, List<AddedMetric> addedMetrics, bool upgradeApproved);
        void FinishAssistantCoachRegistration(string userName, string password, int userId, string emailValidationToken);
        void GenerateUserRoles(Guid userToken);
        List<b.Athlete> GetAllAthletes(Guid userToken);
        Tuple<List<b.Athlete>, int> GetAllAthletesWithoutProgram(Guid userToken, int pageNumber = 0, int count = 0);
        b.Athlete GetAthlete(Guid userToken);
        b.Athlete GetAthlete(Guid userToken, int athleteId);
        void SendAthleteRegistartion(int athleteId, Guid userToken);
        Task SendCoachRegistrationEmail(int userId);
    }

    public class RosterManager : IRosterManager
    {
        private static string AzureFunctionsBaseURL => ConfigurationManager.AppSettings["AzureFunctionsBaseURL"];

        private IAthleteRepo _athleteRepo { get; set; }
        private ITagRepo<AthleteTag> _athleteTagRepo { get; set; }
        private IProgramRepo _programRepo { get; set; }
        private IWeightRoomRepo _weightRoomRepo { get; set; }
        private IOrganizationRepo _orgRepo { get; set; }
        private IUserRepo _userRepo { get; set; }
        private List<OrganizationRoleEnum> _userRoles { get; set; }

        String StripeKey = Environment.GetEnvironmentVariable("STRIPE_KEY");

        public RosterManager(IAthleteRepo athleteRepo, ITagRepo<AthleteTag> tagRepo, IProgramRepo programRepo, IWeightRoomRepo weightRoomRepo, IOrganizationRepo orgRepo, IUserRepo userRepo)
        {
            _orgRepo = orgRepo;
            _athleteRepo = athleteRepo;
            _athleteTagRepo = tagRepo;
            _athleteTagRepo.InitializeTagRepo(TagEnum.Athlete);
            _userRepo = userRepo;
            _programRepo = programRepo;
            _weightRoomRepo = weightRoomRepo;
        }
        public void GenerateUserRoles(Guid userToken)
        {
            _userRoles = _orgRepo.GetUserRoles(userToken);
        }

        public List<DAL.DTOs.Athlete.AssignedProgramAthleteDTO> CheckAthletesAssignedPrograms(List<int> AthleteIds, Guid UserToken)
        {
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.AssignPrograms)))
            {
                throw new ApplicationException("User Does Not Have Rights To Assign Programs");
            }

            try
            {
                if (!_userRepo.Get(UserToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Assign Athletes");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }
            return _athleteRepo.GetAllAssignedProgramAthletes(AthleteIds);
        }
        public async Task AssignProgramToAthletes(List<int> athleteIds, int programId, Guid UserToken, DateTime startDate)
        {
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.AssignPrograms)))
            {
                throw new ApplicationException("User Does Not Have Rights To Assign Programs");
            }

            try
            {
                if (!_userRepo.Get(UserToken).IsCoach) throw new ApplicationException("Only Coaches Can Create Assign Athletes");
            }
            catch (Exception)
            {
                throw new ApplicationException("The account You are using has an issue; Please LOGOUT and LOG back in");
            }

            //figure out way to fire and forget, its taking too much time for the user
            for (int i = 0; i < athleteIds.Count; i++)
            {
                await new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/AssignProgramSnapShots?code={Config.AssignProgramSnapShots}",
                                 new StringContent(JsonConvert.SerializeObject(new AzureFunctions.SnapshotInfo()
                                 {
                                     ProgramId = programId,
                                     AthleteId = athleteIds[i],
                                     UserToken = UserToken
                                 })));
            }
            
        }
        public async Task CreateAssistantCoach(string firstName, string lastName, string email, Guid userToken)
        {
            if (!_userRoles.Contains(OrganizationRoleEnum.Admin))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Assistant Coaches");
            }
            var existingUser = _userRepo.GetByEmail(email);
            if (existingUser != null) throw new ApplicationException("A User Already Exists With This Email");

            var salt = Guid.NewGuid();//todo: i dont know why we need this. further investigate please
            var tempPassword = Guid.NewGuid().ToString();
            string encryptedPassword = Crypter.Blowfish.Crypt(tempPassword);
            var emailValidationToken = Guid.NewGuid().ToString();
            var createdUser = _userRepo.Get(userToken);
            if (!createdUser.IsCoach) throw new ApplicationException("Only Coachs Can Create Athletes.");
            if (string.IsNullOrEmpty(createdUser.UserName)) throw new ApplicationException("Please Log Out and Log Back In");
            var newUserId = _userRepo.Create(new Models.User.User()
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                IsHeadCoach = false,
                IsCoach = true,
                LockedOut = false,
                IsDeleted = false,
                UserName = $"{firstName}_{lastName}_{email}",
                Password = encryptedPassword,
                ImageContainerName = Guid.NewGuid().ToString(),
                OrganizationId = createdUser.OrganizationId,
                EmailValidationToken = emailValidationToken
            });

           await SendCoachRegistrationEmail(newUserId);

        }
        public async Task SendCoachRegistrationEmail(int userId)
        {
            if (!_userRoles.Contains(OrganizationRoleEnum.Admin))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Assistant Coaches");
            }
            var emailValidationToken = Guid.NewGuid();
            await _userRepo.UpdateUserEmailValidationToken(emailValidationToken, userId);

            var targetCoach = _userRepo.Get(userId);

            var registrationCompleteURL = $"{Config.EmailRegistrationBaseUrl}/AssistantCoachEmailRegistration/{emailValidationToken}/{userId}";

            await new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/AssistantCoachEmailRegistration?code={Config.AssistantCoachEmailCode}",
            new StringContent(JsonConvert.SerializeObject(
            new EmailDetails() { ToEmail = targetCoach.Email, URL = registrationCompleteURL, FirstName = targetCoach.FirstName }), Encoding.UTF8, "application/json"));

        }
        public void FinishAssistantCoachRegistration(string userName, string password, int userId, string emailValidationToken)
        {
            var salt = Guid.NewGuid();//todo: i dont know why we need this. further investigate please

            var user = _userRepo.Get(userName);
            if (!string.IsNullOrEmpty(user.UserName)) throw new ApplicationException("User Name Is Already Taken");

            var targetCoach = _userRepo.Get(userId);
            if (targetCoach.IsEmailValidated) throw new ApplicationException("This User Has Already Been Registered");

            _orgRepo.FinishAssistantCoachRegistration(userName, Crypter.Blowfish.Crypt(password), userId, emailValidationToken);
        }


        public int CreateAthlete(DAL.DTOs.AthleteDTO newAthlete, List<AthleteTag> tagIds, Guid userToken, List<AddedMetric> addedMetrics, bool upgradeApproved)
        {
            var createdUser = _userRepo.Get(userToken);
            var currentStatus = _orgRepo.GetOrganizationAthleteCountStatus(createdUser.OrganizationId);
            if ((currentStatus.Item1 + 1 > currentStatus.Item2) && !upgradeApproved)
            {
                throw new ApplicationException("Adding another Athelte will automatically upgrade your account. You need to approve the account upgrade in order to proceed");
            }


            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.CreateAthletes)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Athletes");
            }

            User existingUser = null;
            if (!string.IsNullOrEmpty(newAthlete.Email))
            {
                _userRepo.GetByEmail(newAthlete.Email);
            }
            if (existingUser != null) throw new ApplicationException("A User Already Exists With This Email");

            var salt = Guid.NewGuid();//todo: i dont know why we need this. further investigate please
            var tempPassword = Guid.NewGuid().ToString();
            string encryptedPassword = Crypter.Blowfish.Crypt(tempPassword);
            var emailValidationToken = Guid.NewGuid().ToString();

            if (!createdUser.IsCoach) throw new ApplicationException("Only Coachs Can Create Athletes.");
            if (string.IsNullOrEmpty(createdUser.UserName)) throw new ApplicationException("Please Log Out and Log Back In");
            var newUserId = _userRepo.Create(new Models.User.User()
            {
                Email = string.IsNullOrEmpty(newAthlete.Email) ? null : newAthlete.Email,
                IsCoach = false,
                LockedOut = false,
                IsDeleted = false,
                IsHeadCoach = false,
                OrganizationId = createdUser.OrganizationId,
                UserName = $"{ newAthlete.FirstName}_{ newAthlete.LastName}_{ newAthlete.Email}",
                Password = encryptedPassword,
                ImageContainerName = Guid.NewGuid().ToString()
            });

            var newAthleteId = _athleteRepo.CreateAthlete(newAthlete.FirstName, newAthlete.LastName, createdUser.Id, newUserId, !string.IsNullOrEmpty(newAthlete.Email) ? emailValidationToken : string.Empty, DateTime.Now, newAthlete.Birthday);
            newAthlete.Id = newAthleteId;
            if (newAthlete.Weight.HasValue || newAthlete.HeightPrimary.HasValue || newAthlete.HeightSecondary.HasValue)
            {
                _athleteRepo.UpdateAthleteHeightWeight(newAthleteId, newAthlete.HeightPrimary, newAthlete.HeightSecondary, newAthlete.Weight);
            }

            AddTagsToAthlete(tagIds, newAthleteId, userToken);

            if (addedMetrics != null)
            {
                addedMetrics.ForEach(x => _athleteRepo.AddStandAloneMetric(newAthleteId, x.MetricId, x.Value, createdUser.Id, x.CompletedDate));
            }
            if (!string.IsNullOrEmpty(newAthlete.Email))
            {
                SendAthleteRegistartion(emailValidationToken, newAthlete);
            }
            return newAthlete.Id;
        }

        public void SendAthleteRegistartion(int athleteId, Guid userToken)
        {
            var activeUser = _userRepo.Get(userToken);
            if (!activeUser.IsCoach) throw new ApplicationException("Only Coachs Can Create Athletes.");

            var targetAthlete = _athleteRepo.GetAthlete(athleteId);
            var athleteUserAcccount = _userRepo.Get(targetAthlete.AthleteUserId);
            var org = _orgRepo.GetOrg(athleteUserAcccount.OrganizationId);
            if (activeUser.OrganizationId != targetAthlete.OrganizationId) throw new ApplicationException("You Can Only Do This Action For Athletes Within Your Organization");

            var newEmailValidationToken = Guid.NewGuid().ToString();
            _athleteRepo.UpdateEmailValidationToken(newEmailValidationToken, targetAthlete.Id);

            SendAthleteRegistartion(newEmailValidationToken, new AthleteDTO() { Id = targetAthlete.Id, Email = athleteUserAcccount.Email, FirstName = athleteUserAcccount.FirstName, OrganizationName = org.Name });
        }
        private void SendAthleteRegistartion(string emailValidationToken, AthleteDTO targetAthlete)
        {
            var registrationCompleteURL = $"{Config.EmailRegistrationBaseUrl}/AthleteEmailVerification/{emailValidationToken}/{targetAthlete.Id}";

            var res = new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/SendCreateAthleteEmail?code={Config.SendCreateAthleteEmail}",
            new StringContent(JsonConvert.SerializeObject(
            new EmailDetails() { ToEmail = targetAthlete.Email, URL = registrationCompleteURL, FirstName = targetAthlete.FirstName, OrganizationName = targetAthlete.OrganizationName }), Encoding.UTF8, "application/json")).Result;
        }
        public void AddProfilePictureToAthlete(int athleteId, int picId, Guid userToken)
        {
            _athleteRepo.AddProfilePictureToAthlete(athleteId, picId, userToken);
        }
        public List<b.Athlete> GetAllAthletes(Guid userToken)
        {
            var weightRoomAccount = _weightRoomRepo.Get(userToken);
            var allAthletes = new List<Models.Athlete.Athlete>();
            var allMappings = new List<AthleteWithTagsDTO>();
            if (weightRoomAccount != null)
            {
                allAthletes = _athleteRepo.GetAllAthletes(weightRoomAccount.OrganizationId);
                allMappings = _athleteRepo.GetAllAthletesTagMappings(weightRoomAccount.OrganizationId);
            }
            else
            {
                allAthletes = _athleteRepo.GetAllAthletes(userToken);
                allMappings = _athleteRepo.GetAllAthletesTagMappings(userToken);
            }

            var ret = new List<b.Athlete>();

            foreach (var e in allAthletes)
            {
                var mapping = allMappings.FirstOrDefault(x => x.AthleteId == e.Id);
                var user = _userRepo.Get(e.AthleteUserId);
                ret.Add(CreateBLAthleteModel(e, mapping, _athleteRepo.GetLatestAthleteBioMetrics(e.Id), user.Email));
            }
            return ret;
        }
        public Tuple<List<b.Athlete>, int> GetAllAthletesWithoutProgram(Guid userToken, int pageNumber = 0, int count = 0)
        {
            var weightRoomAccount = _weightRoomRepo.Get(userToken);
            var allAthletes = new List<Models.Athlete.Athlete>();
            var allMappings = new List<AthleteWithTagsDTO>();

            allAthletes = _athleteRepo.GetAllAtheltesWithoutProgram(userToken).OrderBy(x => x.Id).ToList();
            var totalAthleteCount = allAthletes.Count();
            if (count > 0)
            {
                allAthletes = allAthletes.OrderBy(x => x.Id).Skip(pageNumber * count).Take(count).ToList();
            }

            allMappings = _athleteRepo.GetAllAthletesTagMappings(userToken);

            var ret = new List<b.Athlete>();

            foreach (var e in allAthletes)
            {
                var mapping = allMappings.FirstOrDefault(x => x.AthleteId == e.Id);
                var user = _userRepo.Get(e.AthleteUserId);
                ret.Add(CreateBLAthleteModel(e, mapping, _athleteRepo.GetLatestAthleteBioMetrics(e.Id), user.Email));
            }
            return new Tuple<List<b.Athlete>, int>(ret, totalAthleteCount);
        }
        public b.Athlete GetAthlete(Guid userToken, int athleteId)
        {
            var heightWeight = _athleteRepo.GetLatestAthleteBioMetrics(athleteId);
            var e = _athleteRepo.GetAthlete(athleteId, userToken);
            var mapping = _athleteRepo.GetAllAthletesTagMappings(userToken).FirstOrDefault(x => x.AthleteId == e.Id);
            var user = _userRepo.Get(e.AthleteUserId);

            return CreateBLAthleteModel(e, mapping, heightWeight, user.Email);
        }
        public b.Athlete GetAthlete(Guid userToken)
        {

            var e = _athleteRepo.GetAthlete(userToken);
            var heightWeight = _athleteRepo.GetLatestAthleteBioMetrics(e.Id);
            var mapping = _athleteRepo.GetAllAthletesTagMappings(userToken).FirstOrDefault(x => x.AthleteId == e.Id);
            var user = _userRepo.Get(e.AthleteUserId);
            return CreateBLAthleteModel(e, mapping, heightWeight, user.Email);
        }

        public void AddTagsToAthlete(List<AthleteTag> tagIds, int athleteId, Guid createdUserGuid)
        {
            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.CreateAthletes) || _userRoles.Contains(OrganizationRoleEnum.ModifyAthletes)))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Athletes");
            }
            var targetAthlete = _athleteRepo.GetAthlete(athleteId, createdUserGuid);
            if (targetAthlete == null) return;


            _athleteTagRepo.DeleteAssociatedTags(athleteId);
            _athleteTagRepo.AddAssociatedTags(tagIds, athleteId);
        }
        private void UpdateStripeForOrganization(string SessionId)
        {
            StripeConfiguration.ApiKey = StripeKey;
            var service = new SessionService();
            Session session = service.Get(SessionId);
            if (session != null && session.CustomerId != null)
            {
                try { _orgRepo.UpdateOrganizationWithStripeData(session.CustomerId, SessionId); } catch (Exception e) { throw e; }
            }
        }
        private b.Athlete CreateBLAthleteModel(Athlete targetAthlete, AthleteWithTagsDTO mapping, AthleteHeightWeight heightWeight, string email)
        {
            return new b.Athlete()
            {
                Id = targetAthlete.Id,
                FirstName = targetAthlete.FirstName,
                LastName = targetAthlete.LastName,
                ProgramId = targetAthlete.AssignedProgramId,
                Email = email,
                Tags = mapping == null ? new List<b.Tag>() : mapping.Tags.Select(x => new b.Tag() { Id = x.Id, Name = x.Name }).ToList(),
                ProfilePicture = targetAthlete.ProfilePicture,
                IsDeleted = targetAthlete.IsDeleted,
                Weight = heightWeight?.Weight,
                HeightPrimary = heightWeight?.HeightPrimary,
                HeightSecondary = heightWeight?.HeightSecondary,
                Birthday = targetAthlete?.Birthday,
                UserId = targetAthlete.AthleteUserId,
                ValidatedEmail = targetAthlete.ValidatedEmail
            };
        }
    }
    public class StripeUpgradeParams
    {
        public int OrganizationId { get; set; }
        public int NewPlanId { get; set; }
    }
}
