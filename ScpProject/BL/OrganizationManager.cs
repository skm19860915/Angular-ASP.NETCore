using AzureFunctions;
using BL.BusinessObjects.Organization;
using DAL.DTOs;
using DAL.DTOs.Organization;
using DAL.Repositories;
using Models.Enums;
using Models.Organization;
using Newtonsoft.Json;
using Stripe;
using Stripe.Checkout;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace BL
{
    public interface IOrganizationManager
    {
        void AddImage(int pictureId, Guid userToken);
        void AddPrimaryColor(string primaryColorHex, Guid userToken);
        void AddPrimaryFontColor(string primaryFontColorHex, Guid userToken);
        void AddSecondaryColor(string secondaryColorHex, Guid userToken);
        void AddSecondaryFontColor(string secondaryColorHex, Guid userToken);
        void ArchiveCoach(int coachId, Guid headCoachToken);
        void AssignRole(int assistantCoach, OrganizationRoleEnum newRole, Guid assigner);
        void CancelSubscription(string stripeGuid);
        bool CheckOrganizationExists(string name);
        int CreateOrganization(string name);
        void DeleteOrganization(int orgId);
        void DeleteOrganization(string name);
        List<AssitantCoach> GetAllCoaches(Guid userToken, bool includeHeadCoach = false, bool excludeSelf = false);
        List<Role> GetAllRoles();
        BusinessObjects.OrganizationViewModel GetOrg(int orgId, bool wipePersonalInfo = true);
        Organization GetOrg(string stripeGuid);
        OrganizationAthleteCount GetOrganizationAthleteCountStatus(Guid currentUser);
        SubscriptionInfo GetSubscriptionInfo(Guid userToken);
        WeightRoomAccount GetWeightRoomUser(Guid userToken);
        Task<bool> ManualUpgradeSubscription(Guid userToken, int newPlanId);
        void MarkCardExpiring(string stripeGuid);
        void MarkStripeFailedToProcess(string stripeGuid);
        void ResetCardInfo(string stripeGuid, string paymentMethodId);
        void ResetSubscription(string orgId);
        void UnAssignRole(int assistantCoach, OrganizationRoleEnum newRole, Guid unassigner);
        void UpdateOrgname(string orgName, Guid createdUserToken);
        void UpdateStripeForOrganization(string SessionId);
        void UpdateStripeForOrganization(string customerId, int orgId, int subscriptionPlanId);
        void UpdateStripeSessionForOrganization(int OrganizationId, string SessionId, int planId);
        Task<bool> UpgradeSubscription(Guid userToken);
        List<OrganizationRoleEnum> GetUserRoles(Guid userToken);
    }

    public class OrganizationManager : IOrganizationManager
    {
        string StripeKey => ConfigurationManager.AppSettings["StripeKey"];

        private IMultimediaRepo _mmRepo { get; set; }
        private IOrganizationRepo _orgRepo { get; set; }
        private IWeightRoomRepo _weightRoomRepo { get; set; }
        private List<OrganizationRoleEnum> _userRoles { get; set; }
        private IUserRepo _userRepo { get; set; }
        private ISubscriptionRepo _subRepo { get; set; }

        public OrganizationManager(IOrganizationRepo orgRepo, IWeightRoomRepo weightRoom, IUserRepo userRepo, IMultimediaRepo mmrepo, ISubscriptionRepo subRepo)
        {
            _orgRepo = orgRepo;
            _weightRoomRepo = weightRoom;
            _userRepo = userRepo;
            _mmRepo = mmrepo;
            _subRepo = subRepo;

        }
        public List<OrganizationRoleEnum> GetUserRoles(Guid userToken)
        {
            return _orgRepo.GetUserRoles(userToken);
        }
        private void GenerateUserRoles(Guid userToken)
        {
            _userRoles = _orgRepo.GetUserRoles(userToken);
        }
        public void UpdateOrgname(string orgName, Guid createdUserToken)
        {
            if (!_userRepo.Get(createdUserToken).IsHeadCoach) return;
            _orgRepo.UpdateOrganizationName(orgName, createdUserToken);
        }
        public void AddImage(int pictureId, Guid userToken)
        {
            if (!_userRepo.Get(userToken).IsHeadCoach) return;
            _orgRepo.AddImage(pictureId, userToken);
        }
        public void AddPrimaryColor(string primaryColorHex, Guid userToken)
        {
            if (!_userRepo.Get(userToken).IsHeadCoach) return;
            _orgRepo.AddPrimaryColor(primaryColorHex, userToken);
        }
        public void AddSecondaryColor(string secondaryColorHex, Guid userToken)
        {
            if (!_userRepo.Get(userToken).IsHeadCoach) return;
            _orgRepo.AddSecondaryColor(secondaryColorHex, userToken);
        }
        public void AddPrimaryFontColor(string primaryFontColorHex, Guid userToken)
        {
            if (!_userRepo.Get(userToken).IsHeadCoach) return;
            _orgRepo.AddPrimaryFontColor(primaryFontColorHex, userToken);
        }
        public void AddSecondaryFontColor(string secondaryColorHex, Guid userToken)
        {
            if (!_userRepo.Get(userToken).IsHeadCoach) return;
            _orgRepo.AddSecondaryFontColor(secondaryColorHex, userToken);
        }
        public SubscriptionInfo GetSubscriptionInfo(Guid userToken)
        {
            var user = _userRepo.Get(userToken);
            var allsubs = _orgRepo.GetSubscriptionInfo();
            var currentOrg = _orgRepo.GetOrg(user.OrganizationId);

            var currentSub = allsubs.FirstOrDefault(x => x.Id == currentOrg.CurrentSubscriptionPlanId);
            var nextLevelSub = allsubs.Where(x => x.AthleteCount > currentSub.AthleteCount).OrderBy(y => y.AthleteCount).FirstOrDefault();
            nextLevelSub = nextLevelSub ?? new Models.Payment.SubscriptionType();
            return new SubscriptionInfo()
            {
                CurrentPlanId = currentSub.Id,
                CurrentSubPlanCost = currentSub.Cost,
                CurrentSubPlan = currentSub.Name,
                CurrentSubAthleteNumber = currentSub.AthleteCount,
                CurrentSubStripePlan = currentSub.StripeSubscriptionGuid,
                NextSubPlan = nextLevelSub.Name,
                NextSubPlanCost = nextLevelSub.Cost,
                NextSubPlanCostAthleteNumber = nextLevelSub.AthleteCount,
                NewPlanId = nextLevelSub.Id,
                NextSubStripePlan = nextLevelSub.StripeSubscriptionGuid
            };

        }

        public async Task<bool> ManualUpgradeSubscription(Guid userToken, int newPlanId)
        {
            GenerateUserRoles(userToken);

            if (!(_userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Upgrade/DownGrade Accounts, Only the Account ADMIN may do this");
            }
            var approver = _userRepo.Get(userToken);
            var subInfo = GetSubscriptionInfo(userToken);
            var orgInfo = _orgRepo.GetOrg(approver.OrganizationId);
            var targetSubInfo = _subRepo.GetSubscription(newPlanId);

            var tuple = _orgRepo.GetOrganizationAthleteCountStatus(approver.OrganizationId);
            var athletecount = new OrganizationAthleteCount() { MaxAthletes = tuple.Item2, TotalAthletes = tuple.Item1 };

            if (athletecount.TotalAthletes > targetSubInfo.AthleteCount)
            {
                throw new Exception("Cannot Change Subscription Plan, you have more athletes than the subscription you chose allows");
            }

            var res = await new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/StripeUpgrade?code={Config.UpgradeStripeAccount}",
             new StringContent(JsonConvert.SerializeObject(
              new StripeUpgradeInfo() { CustomerStripeId = orgInfo.StripeGuid, NewPlanStripeGuid = targetSubInfo.StripeSubscriptionGuid, OldPlanStripeGuid = subInfo.CurrentSubStripePlan }), Encoding.UTF8, "application/json"));
            if (res.IsSuccessStatusCode)
            {
                _orgRepo.UpdateOrganizationPlan(approver.OrganizationId, targetSubInfo.Id);
                _orgRepo.AddSubscriptionAudit(approver, subInfo.CurrentPlanId, targetSubInfo.Id);
                return true;
            }
            else
            {
                throw new Exception("Couldnt auto upgrade");
            }
        }
        public async Task<bool> UpgradeSubscription(Guid userToken)
        {
            GenerateUserRoles(userToken);
            if (!(_userRoles.Contains(OrganizationRoleEnum.CreateAthletes) || _userRoles.Contains(OrganizationRoleEnum.Admin)))
            {
                throw new ApplicationException("User Does Not Have Rights To Create Exercises");
            }

            var approver = _userRepo.Get(userToken);
            var subInfo = GetSubscriptionInfo(userToken);
            var orgInfo = _orgRepo.GetOrg(approver.OrganizationId);



            var res = await new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/StripeUpgrade?code={Config.UpgradeStripeAccount}",
             new StringContent(JsonConvert.SerializeObject(
              new StripeUpgradeInfo() { CustomerStripeId = orgInfo.StripeGuid, NewPlanStripeGuid = subInfo.NextSubStripePlan, OldPlanStripeGuid = subInfo.CurrentSubStripePlan }), Encoding.UTF8, "application/json"));
            if (res.IsSuccessStatusCode)
            {
                _orgRepo.UpdateOrganizationPlan(approver.OrganizationId, subInfo.NewPlanId);
                _orgRepo.AddSubscriptionAudit(approver, subInfo.CurrentPlanId, subInfo.NewPlanId);
                return true;
            }
            else
            {
                throw new Exception("Couldnt auto upgrade");
            }
        }


        public bool CheckOrganizationExists(string name)
        {
            return _orgRepo.DoesOrganizationExist(name);
        }
        public WeightRoomAccount GetWeightRoomUser(Guid userToken)
        {
            var user = _userRepo.Get(userToken);
            GenerateUserRoles(userToken);
            var ret = _weightRoomRepo.Get(user.OrganizationId);//the user token that is being passed in is the logged in user. If the logged in user
            //is the weight room account then this code will get nothing. So below if ret is null that means that the _userRepo.get(usertoken) failed 
            //because their isnt a user token available. Meaning 1 of 2 things, 1) that the userToken Is a weightRoom User Token, 2) the token got corrupted.
            //so if the user is null then the user.organizationid is bad that means the user wasnt found and the weight room user is logging in

            if (ret == null)
            {
                ret = _weightRoomRepo.Get(userToken);
            }
            if (ret == null && !(_userRoles.Contains(OrganizationRoleEnum.Admin) || _userRoles.Contains(OrganizationRoleEnum.WeightRoomView)))
            {
                throw new ApplicationException("User Does Not Have Rights To Go To Weight Room View");
            }


            if (ret == null)
            {
                ret = new WeightRoomAccount()
                {
                    OrganizationId = user.OrganizationId,
                    Name = Guid.NewGuid().ToString(),
                    Token = Guid.NewGuid().ToString()
                };
                _weightRoomRepo.CreateAccount(ret.OrganizationId, ret.Name, ret.Token);
            }
            return ret;
        }
        private void CreateWeighRoomUser(int orgId, Guid createUserToken)
        {
            _weightRoomRepo.CreateAccount(orgId, Guid.NewGuid().ToString(), Guid.NewGuid().ToString());
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgId"></param>
        /// <returns>Tuple(CurrentAthleteCount,MaxAthleteCount)</currentAthleteCount></returns>
        public OrganizationAthleteCount GetOrganizationAthleteCountStatus(Guid currentUser)
        {

            var athleteCount = _orgRepo.GetOrganizationAthleteCountStatus(_userRepo.Get(currentUser).OrganizationId);
            return new OrganizationAthleteCount() { MaxAthletes = athleteCount.Item2, TotalAthletes = athleteCount.Item1 };
        }
        public void ArchiveCoach(int coachId, Guid headCoachToken)
        {
            var coachRoles = _orgRepo.GetUserRoles(headCoachToken);
            var coachToDelete = _userRepo.Get(coachId);
            var headCoach = _userRepo.Get(headCoachToken);
            if (!coachRoles.Contains(OrganizationRoleEnum.Admin) && coachToDelete.OrganizationId == headCoach.OrganizationId)
            {
                throw new ApplicationException("User Does Not Have Rights To Archive Coaches");
            }
            _orgRepo.DeleteCoach(coachId, headCoach.Id);
        }
        public void DeleteOrganization(int orgId)
        {
            _orgRepo.DeleteOrganizationFromBadRegistration(orgId);
        }
        public int CreateOrganization(string name)
        {
            return _orgRepo.CreateOrganization(name);
        }

        public void DeleteOrganization(string name)
        {
            _orgRepo.DeleteOrganization(name);
        }
        public List<Role> GetAllRoles()
        {
            return _orgRepo.GetRoles();
        }
        public List<AssitantCoach> GetAllCoaches(Guid userToken, bool includeHeadCoach = false, bool excludeSelf = false)
        {
            var self = _userRepo.Get(userToken);
            var ret = _orgRepo.GetAllCoaches(userToken, includeHeadCoach);
            if (excludeSelf)
            {
                ret = ret.Where(x => x.Id != self.Id).ToList();
            }
            return ret;
        }
        public void AssignRole(int assistantCoach, OrganizationRoleEnum newRole, Guid assigner)
        {
            GenerateUserRoles(assigner);
            if (!_userRoles.Contains(OrganizationRoleEnum.Admin))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Coaches Roles");
            }
            var user = _userRepo.Get(assigner);
            _orgRepo.AssignRole(assistantCoach, newRole, user.OrganizationId, user.Id);
        }
        public void UnAssignRole(int assistantCoach, OrganizationRoleEnum newRole, Guid unassigner)
        {
            GenerateUserRoles(unassigner);
            if (!_userRoles.Contains(OrganizationRoleEnum.Admin))
            {
                throw new ApplicationException("User Does Not Have Rights To Modify Coaches Roles");
            }
            var user = _userRepo.Get(unassigner);
            _orgRepo.RemoveRole(assistantCoach, newRole, user.OrganizationId);

        }
        public BusinessObjects.OrganizationViewModel GetOrg(int orgId, bool wipePersonalInfo = true)
        {
            if (orgId == 0) return new BusinessObjects.OrganizationViewModel();
            var targetOrg = this._orgRepo.GetOrg(orgId);
            var ret = new BusinessObjects.OrganizationViewModel();
            if (wipePersonalInfo)
            {
                targetOrg.StripeGuid = string.Empty;//wipe out personal info because this is being sent down the wire
            }

            ret.Org = targetOrg;
            if (targetOrg.ProfilePictureId.HasValue)
            {
                var targetPic = this._mmRepo.GetPicture(targetOrg.ProfilePictureId.Value);
                ret.profilePictureURL = targetPic.URL + (targetPic.Profile ?? targetPic.Thumbnail ?? targetPic.FileName);
                ret.thumbnailPictureURL = targetPic.URL + (targetPic.Thumbnail ?? targetPic.Profile ?? targetPic.FileName);
            }
            return ret;
        }
        public Organization GetOrg(string stripeGuid)
        {
            return _orgRepo.GetOrg(stripeGuid);
        }
        public void UpdateStripeForOrganization(string SessionId)
        {
            StripeConfiguration.ApiKey = StripeKey;
            var service = new SessionService();
            Session session = service.Get(SessionId);
            if (session != null && session.CustomerId != null)
            {
                //TODO : This needs to be wrapped in our role management system. In addition, create a role for this
                try { _orgRepo.UpdateOrganizationWithStripeData(session.CustomerId, SessionId); } catch (Exception e) { throw e; }
            }
        }
        public void UpdateStripeForOrganization(string customerId, int orgId, int subscriptionPlanId)
        {
            _orgRepo.UpdateOrganizationWithStripeData(customerId, orgId, subscriptionPlanId);
        }

        public void MarkStripeFailedToProcess(string stripeGuid)
        {
            _orgRepo.ChargeFailed(stripeGuid);
        }
        public void MarkCardExpiring(string stripeGuid)
        {
            _orgRepo.CardExpiring(stripeGuid);
        }

        public void CancelSubscription(string stripeGuid)
        {
            _orgRepo.CancelSubscription(stripeGuid);
        }
        public void ResetSubscription(string orgId)
        {
            _orgRepo.ResetSubscription(orgId);
        }
        public void UpdateStripeSessionForOrganization(Int32 OrganizationId, string SessionId, int planId)
        {
            try { _orgRepo.UpdateOrganizationWithSessionData(OrganizationId, SessionId, planId); } catch (Exception e) { throw e; }
        }
        public void ResetCardInfo(string stripeGuid, string paymentMethodId)
        {
            _orgRepo.ResetCardInfo(stripeGuid, paymentMethodId);
        }
    }
}
