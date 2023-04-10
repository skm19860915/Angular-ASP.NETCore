using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BL;
using BL.BusinessObjects.Organization;
using Controllers.ViewModels.Organization;
using DAL.DTOs;
using DAL.DTOs.Organization;
using Models.Enums;

namespace Controllers.Controllers
{
    [RoutePrefix("api/Organization")]
    public class OrganizationController : ApiController
    {
        private IOrganizationManager _orgMan;
        private IUserManager _uMan;
        public OrganizationController(IOrganizationManager orgMan, IUserManager userMan )
        {
            _orgMan = orgMan;
            _uMan = userMan;
        }
        [HttpPost, Route("UpdateOrganization")]
        public HttpResponseMessage UpdateOrganization([FromBody] OrganizationVM orgInfo)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
           
            try
            {
                _orgMan.AddPrimaryColor(orgInfo.PrimaryColorHex, userGuid);
                _orgMan.AddSecondaryColor(orgInfo.SecondaryColorHex, userGuid);
                _orgMan.AddPrimaryFontColor(orgInfo.PrimaryFontColorHex, userGuid);
                _orgMan.AddSecondaryFontColor(orgInfo.SecondaryFontColorHex, userGuid);
                _orgMan.UpdateOrgname(orgInfo.Name, userGuid);

                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {

                return Request.CreateResponse(HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpGet, Route("GetLoggedInUsersOrg")]
        public BL.BusinessObjects.OrganizationViewModel GetLoggedInUsersOrg()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            var targetuser = _uMan.GetUserDetails(userGuid);
            return  _orgMan.GetOrg(targetuser.OrganizationId);
        }
        [HttpGet, Route("GetOrg/{id:int}")]
        public BL.BusinessObjects.OrganizationViewModel GetOrg(int id)
        {
            return  _orgMan.GetOrg(id);

        }
        [HttpPost, Route("CheckOrganizationExists")]
        public bool CheckOrganizationExists(orgName name)
        {
            return  _orgMan.CheckOrganizationExists(name.Name);
        }
        [HttpPost, Route("UpgradeOrganizationSubscription")]
        public async Task<bool> UpgradeOrganizationSubscription()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return await  _orgMan.UpgradeSubscription(userGuid);
        }
        [HttpPost, Route("ManualOrganizationSubscription/{id:int}")]
        public async Task<bool> ManualOrganizationSubscription(int id)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return await  _orgMan.ManualUpgradeSubscription(userGuid, id);
        }
        [HttpGet, Route("GetWeightRoomToken/")]
        public string GetWeightRoomToken()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return  _orgMan.GetWeightRoomUser(userGuid).Token;
        }
        [HttpGet, Route("GetAthleteCount")]
        public OrganizationAthleteCount GetAthleteCount()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return  _orgMan.GetOrganizationAthleteCountStatus(userGuid);
        }
        [HttpGet, Route("GetSubscriptionInfo")]
        public SubscriptionInfo GetSubscriptionInfo()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return  _orgMan.GetSubscriptionInfo(userGuid);

        }
        [HttpPost, Route("DeleteOrganization/{orgId:int}")]
        public void DeleteOrganization(int orgId)
        {
             _orgMan.DeleteOrganization(orgId);
        }
        [HttpPost, Route("ArchiveCoach/{coachId:int}")]
        public void ArchiveCoach(int coachId)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
             _orgMan.ArchiveCoach(coachId, userGuid);
        }
        [HttpPost, Route("CreateOrganization")]
        public HttpResponseMessage CreateOrganization([FromBody] orgName name)
        {
            return Request.CreateResponse(HttpStatusCode.OK,  _orgMan.CreateOrganization(name.Name));
        }
        [HttpPost, Route("DeleteOrganization/{name:alpha}")]
        public void DeleteOrganization(string name)
        {
             _orgMan.DeleteOrganization(name);
        }
        [HttpGet, Route("GetAllRoles")]
        public List<Role> GetAllRoles()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return  _orgMan.GetAllRoles();
        }
        [HttpPost, Route("GetAllCoaches")]
        public List<AssitantCoach> GetAllCoaches()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return  _orgMan.GetAllCoaches(userGuid, true);
        }
        [HttpPost, Route("GetAllCoachesButSelf")]
        public List<AssitantCoach> GetAllCoachesButSelf()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return  _orgMan.GetAllCoaches(userGuid, true, true);
        }
        [HttpPost, Route("GetAllNonHeadCoaches")]
        public List<AssitantCoach> GetAllNonHeadCoaches()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return  _orgMan.GetAllCoaches(userGuid, false);
        }
        [HttpPost, Route("AssignRoles")]
        public void AssignRoles(RoleDTO newRole)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
             _orgMan.AssignRole(newRole.UserId, (OrganizationRoleEnum)newRole.RoleId, userGuid);
        }
        [HttpPost, Route("UnAssignRoles")]
        public void UnAssignRoles(RoleDTO roleToRemove)
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
             _orgMan.UnAssignRole(roleToRemove.UserId, (OrganizationRoleEnum)roleToRemove.RoleId, userGuid);
        }

        [Route("UpdateStripeForOrganization"), HttpGet]
        public string UpdateStripeGuid(string SessionId)
        {
            try {  _orgMan.UpdateStripeForOrganization(SessionId); return "Success"; } catch (Exception e) { throw e; }

        }
    }
    public class orgName
    {
        public string Name { get; set; }
    }

}
