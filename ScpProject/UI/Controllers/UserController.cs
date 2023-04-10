using System;
using System.Web.Http;
using BL;
using Controllers.ViewModels.Login;
using BL.CustomExceptions;
using Controllers.ViewModels;
using System.Net.Http;
using System.Net;
using System.Linq;
using System.Web.Configuration;
using System.Configuration;
using System.Collections.Generic;

namespace Controllers.Controllers
{
    public class oneTime
    {
        public string OrgName { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string UserName { get; set; }
        public string EmailValidationToken { get; set; }
        public int UserId { get; set; }
    }
    [RoutePrefix("api/User")]
    public class UserController : ApiController
    {
        private static string EmailRegistrationBaseUrl => ConfigurationManager.AppSettings["EmailregistrationBaseUrl"];


        private IUserManager _uMan { get; set; }
        private IRosterManager _rosterManger;
        private IOrganizationManager _orgManager;
        public UserController(IUserManager userManager, IRosterManager rosterManager, IOrganizationManager organizationManager)
        {
            _uMan = userManager;
            _rosterManger = rosterManager;
            _orgManager = organizationManager;

        }
        [Route("UserNameInUse"), HttpPost]
        public bool UserNameInUse(UserNameCheck user)
        {
            return _uMan.CheckUserNameInUse(user.userName);
        }
        [Route("EmailInUse"), HttpPost]
        public bool EmailInUse(EmailCheck email)
        {
            return _uMan.CheckEmailInUse(email.Email);
        }

        [Route("Logout"), HttpPost]
        public void Logout()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            _uMan.Logout(userGuid);
        }
        [Route("FinishResettingPassword"), HttpPost]
        public void FinishResetPassword([FromBody] NewPasswordResetInfo newUserInfo)
        {
            _uMan.FinishPasswordReset(newUserInfo.Password, newUserInfo.ConfirmPassword, newUserInfo.ValidationToken, newUserInfo.UserId);
        }
        [Route("ResetPassword"), HttpPost]//this doesnt reset the password it just generates the email
        public void Resetpassword([FromBody] string emailAddress)
        {
            _uMan.ResetPassword(emailAddress);
        }
        [Route("ResendUserName"), HttpPost]
        public void ResendUserName([FromBody] string emailAddress)
        {
            _uMan.ForgotuserName(emailAddress);
        }
        [HttpPost, Route("OneTimeRegisterHeadCoach")]
        public HttpResponseMessage OneTimeRegisterHeadCoach([FromBody] oneTime time)
        {
            if (time.Password != time.ConfirmPassword) throw new ApplicationException("Passwords Do Not Match");
            _uMan.OneTimeRegisterHeadCoach(time.OrgName, time.Password, time.UserName, Guid.Parse(time.EmailValidationToken), time.UserId);
            return Request.CreateResponse(HttpStatusCode.OK);
        }
        [HttpGet, Route("VerifyToken")]
        public HttpResponseMessage VerifyToken()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return Request.CreateResponse(HttpStatusCode.OK, _uMan.VerifyToken(userGuid));
        }


        [Route("Login"), HttpPost]
        public HttpResponseMessage Login(LoginAttempt loginAttempt)
        {
            try
            {
               
                var ret = _uMan.Login(loginAttempt.UserName, loginAttempt.Password);
                var targetUser = _uMan.GetUserDetails(ret.Item1);
                var roles = _orgManager.GetUserRoles(ret.Item1);//lazy ass george using tuples brownie points to whomever fixes this (12 pack of your choice)
                var targetOrg =  _orgManager.GetOrg(targetUser.OrganizationId).Org;

                return Request.CreateResponse(HttpStatusCode.OK, new Login()
                {
                    UserToken = ret.Item1,
                    IsCoach = ret.Item2,
                    Name = ret.Item3,
                    Email = ret.Item4,
                    IsCustomer = targetOrg.IsCustomer,
                    IsHeadCoach = targetUser.IsHeadCoach,
                    Roles = new List<Boolean>() { roles.Contains(Models.Enums.OrganizationRoleEnum.Admin), false }
                });
            }
            catch (UserNotFoundException ex)
            {

                return Request.CreateResponse(HttpStatusCode.Conflict, ex.Message);
            }
            catch (FailedLoginException ex)
            {

                return Request.CreateResponse(HttpStatusCode.Conflict, ex.Message);
            }
        }

        [Route("GetLoggedInAthlete"), HttpGet]
        public HttpResponseMessage GetLoggedInAthletegin()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);
            return Request.CreateResponse(HttpStatusCode.OK, _rosterManger.GetAthlete(userGuid));
        }

        [Route("UserEmailVerification/{emailToken:alpha}")]
        public HttpResponseMessage FinishUserRegistration(string emailToken)
        {
            var isUserValidated = _uMan.FinishUserRegistration(emailToken);
            return Request.CreateResponse(HttpStatusCode.OK);
        }

        [Route("UpdatedVisited/{Field}"), HttpGet]
        public HttpResponseMessage UpdateVistiedCount(int Field)
        {
            string token = Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value;
            var userGuid = Guid.Parse(token);
            try
            { _uMan.UpdateVisitedCount(userGuid, Field); return Request.CreateResponse(HttpStatusCode.OK); }
            catch (Exception e) { return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message); }
        }


        [Route("GetVisited"), HttpGet]
        public HttpResponseMessage GetVisitedStatus()
        {
            string token = Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value;
            var userGuid = Guid.Parse(token);
            try
            { return Request.CreateResponse(HttpStatusCode.OK, _uMan.GetVisitedCount(userGuid)); }
            catch (Exception e) { return Request.CreateResponse(HttpStatusCode.BadRequest, e.Message); }
        }
        [Route("UpdatePassword"), HttpPost]
        public HttpResponseMessage UpdatePassword([FromBody] UpdatePasswordVM newPassword)
        {
            string token = Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value;
            var userGuid = Guid.Parse(token);

            try
            {
               _uMan.UpdatePassword(newPassword.Password, newPassword.ConfirmPassword, userGuid);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, ex.Message);
            }

        }

        [Route("Register"), HttpPost]
        public HttpResponseMessage Register([FromBody] Register regInfo)
        {
            try
            {
                _uMan.CreateHeadCoach(new Models.User.User() { OrganizationId = regInfo.OrgId, Email = regInfo.Email, Password = regInfo.Password, UserName = regInfo.UserName, IsCoach = true, FirstName = regInfo.FirstName, LastName = regInfo.LastName }, regInfo.ConfirmPassword);
                //var ret = _uMan.Login(regInfo.UserName, regInfo.Password);
                //return Request.CreateResponse(HttpStatusCode.OK, new Login() { UserToken = ret.Item1, IsCoach = ret.Item2 });
                return Request.CreateResponse(HttpStatusCode.OK, new Login() { UserToken = Guid.NewGuid(), IsCoach = true });
            }
            catch (MismatchingPasswordsException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, ex.Message);
            }
            catch (UserAlreadyExistsException ex)
            {
                return Request.CreateResponse(HttpStatusCode.Conflict, ex.Message);
            }
        }
        [Route("StripeCart/{organizationId:int}/{subscriptionPlan:int}"), HttpGet]
        public IHttpActionResult StripeCart(int organizationId, int subscriptionPlan)
        {
            try
            {
                return Ok(_uMan.CreateStripeUserSession(organizationId, subscriptionPlan));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to Create Session " + ex.ToString()));
            }
        }

        //[Route("StripeCart/{organizationId:int}/{subscriptionPlan:int}"),HttpGet]
        //public IHttpActionResult StripeCart(int organizationId, int subscriptionPlan, StripeCustomerData stripeCustomerData)
        //{
        //    try
        //    {
        //        return Ok(_uMan.CreateAndSaveStripeData(organizationId, subscriptionPlan, stripeCustomerData));
        //    }
        //    catch (Exception ex)
        //    {
        //        return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to Create Session " + ex.ToString()));
        //    }
        //}

        [Route("UpdateStripeCart"), HttpGet]
        public IHttpActionResult StripeCart()
        {
            try
            {
                string token = Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value;
                var userGuid = Guid.Parse(token);
                return Ok(_uMan.CreateStripeUserSessionForUpdate(userGuid));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to Create Session " + ex.ToString()));
            }
        }


        [Route("StripeCart/Cancel"), HttpPost]
        public IHttpActionResult CancelStripeSubscription(Feedback feedBack)
        {
            try
            {
                string token = Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value;
                var userGuid = Guid.Parse(token);
                return ResponseMessage(Request.CreateResponse(_uMan.CancelSubscription(userGuid, feedBack.feedBack)));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to Create Session " + ex.ToString()));
            }
        }

        [Route("Details"), HttpGet]
        public IHttpActionResult UserDetails()
        {
            try
            {
                string token = Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value;
                var userGuid = Guid.Parse(token);
                return ResponseMessage(Request.CreateResponse(_uMan.GetUserDetails(userGuid)));
            }
            catch (Exception ex)
            {
                return ResponseMessage(Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to Fetch User " + ex.ToString()));
            }
        }

        [Route("UpdateUserInfo"), HttpPost]
        public HttpResponseMessage UpdateUserInfo([FromBody] AccountSettings newUserInfo)
        {
            string token = Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value;
            var userGuid = Guid.Parse(token);

            try
            {
                _uMan.UpdateUser(newUserInfo.FirstName, newUserInfo.LastName, newUserInfo.Email, userGuid);
                return Request.CreateResponse(HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, "Failed to Update User " + ex.ToString());
            }
        }

        //[Route("UpdatePaymentInformation")]
        //public 

        public class AccountSettings
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
        }
        public class NewPasswordResetInfo
        {
            public int UserId { get; set; }
            public string ValidationToken { get; set; }
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }
        public class UpdatePasswordVM
        {
            public string Password { get; set; }
            public string ConfirmPassword { get; set; }
        }
        public class UserNameCheck
        {
            public string userName { get; set; }
        }
        public class Feedback
        {
            public string feedBack { get; set; }
        }
        public class EmailCheck
        {
            public string Email { get; set; }
        }
    }
}