using System;
using DAL.Repositories;
using CryptSharp;
using BL.CustomExceptions;
using Models.User;
using System.Net.Http;
using AzureFunctions;
using Newtonsoft.Json;
using System.Text;
using Stripe;
using Models.Enums;
using System.Collections.Generic;
using Stripe.Checkout;
using System.Configuration;
using Models.Organization;
using BL.BusinessObjects.Token;

namespace BL
{
    public interface IUserManager
    {
        bool CancelSubscription(Guid token, string feedback = "");
        bool CheckEmailInUse(string email);
        bool CheckIfUserNameAlreadyExists(string userName);
        bool CheckUserNameInUse(string userName);
        bool CreateAndSaveStripeData(int organizationId, int subscriptionPlan, StripeCustomerData stripeCustomerData);
        Token CreateCard(StripeCustomerData stripeCustomerData);
        int CreateHeadCoach(User newUser, string confirmedPassword);
        Session CreateStripeUpdateSession(int organizationId, int subscriptionPlan);
        Session CreateStripeUserSession(int organizationId, int subscriptionPlan);
        Session CreateStripeUserSessionForUpdate(Guid guid);
        void FinishPasswordReset(string password, string confirmPassword, string passwordToken, int userId);
        bool FinishUserRegistration(string emailGuid);
        void ForgotuserName(string emailAddress);
        User GetUserDetails(Guid guid);
        UserVisitedStatus GetVisitedCount(Guid token);
        Tuple<Guid, bool, string, string> Login(string userName, string password);
        void Logout(Guid token);
        void OneTimeRegisterHeadCoach(string organizationName, string password, string userName, Guid emailValToken, int userId);
        void ResetPassword(string emailAddress);
        void UpdatePassword(string password, string confirmPassword, Guid userToken);
        void UpdateUser(string firstName, string lastName, string email, Guid userToken);
        User UpdateUserDetails(Guid guid, User user);
        void UpdateVisitedCount(Guid token, int UpdateField);
        VerifyToken VerifyToken(Guid userToken);
    }

    public class UserManager : IUserManager
    {
        private static string AzureFunctionsBaseURL => ConfigurationManager.AppSettings["AzureFunctionsBaseURL"];

        private IUserRepo _userRepo { get; set; }
        private ISubscriptionRepo _SubRepo { get; set; }
        private IWeightRoomRepo _weightRoomRepo { get; set; }
        private static string EmailRegistrationBaseUrl => ConfigurationManager.AppSettings["EmailregistrationBaseUrl"];
        private IAdministrationRepo _adminRepo { get; set; }
        private IOrganizationRepo _orgRepo { get; set; }
        private IAthleteRepo _athleteRepo { get; set; }
        private IOrganizationManager _orgMan { get; set; }
        private IUserTokenRepo _userTokenRepo { get; set; }
        string StripeKey => ConfigurationManager.AppSettings["StripeKey"];

        public UserManager(IUserRepo userRepo, ISubscriptionRepo subRepo, IWeightRoomRepo weightRoomRepo, IAdministrationRepo adminrepo, IOrganizationRepo orgRepo, IAthleteRepo athleteRepo
            , IOrganizationManager orgMan, IUserTokenRepo userTokenRepo)
        {
            _userRepo = userRepo;
            _SubRepo = subRepo;
            _weightRoomRepo = weightRoomRepo;
            _adminRepo = adminrepo;
            _userTokenRepo = userTokenRepo;
            _orgRepo = orgRepo;
            _athleteRepo = athleteRepo;
            _orgMan = orgMan;
        }

        public void UpdateUser(string firstName, string lastName, string email, Guid userToken)
        {
            var targetUserToUpdate = _userRepo.Get(userToken);
            var getUserByEmail = _userRepo.GetByEmail(email);

            if (getUserByEmail != null && targetUserToUpdate.Id != getUserByEmail.Id) throw new ApplicationException("A User Already Exists With This Email");

            _userRepo.UpdateUserInfo(firstName, lastName, email, targetUserToUpdate.Id);
        }
        public bool CheckUserNameInUse(string userName)
        {
            return _userRepo.UserNameInUse(userName);
        }
        public bool CheckEmailInUse(string email)
        {
            return _userRepo.EmailInUse(email);
        }
        public void UpdatePassword(string password, string confirmPassword, Guid userToken)
        {
            var user = _userRepo.Get(userToken);
            if (password != confirmPassword) throw new MismatchingPasswordsException("Your Passwords Didnt Match, Please re-enter your passwords");


            string encryptedPassword = Crypter.Blowfish.Crypt(password);
            _userRepo.UpdateUserPassword(encryptedPassword, user.Id);
        }
        public void FinishPasswordReset(string password, string confirmPassword, string passwordToken, int userId)
        {
            if (password != confirmPassword) throw new MismatchingPasswordsException("Your Passwords Didnt Match, Please re-enter your passwords");
            var PasswordResetInfo = _userRepo.PasswordTokenInfo(passwordToken);

            if (PasswordResetInfo == null) throw new ApplicationException("This Token Is Incorrect");
            if (DateTime.Compare(DateTime.Now.ToUniversalTime(), PasswordResetInfo.ExpiresInUTC) > 0) throw new ApplicationException("This token has expired");
            if (PasswordResetInfo.UserId != userId) throw new ApplicationException("This Token Is Incorrect");

            var salt = Guid.NewGuid();//todo: i dont know why we need this. further investigate please
            string encryptedPassword = Crypter.Blowfish.Crypt(password);
            _userRepo.UpdateUserPassword(encryptedPassword, userId);
        }
        public void ResetPassword(string emailAddress)
        {
            var user = _userRepo.GetByEmail(emailAddress);
            if (user == null) return;
            var passwordResetInfo = new PasswordReset()
            {
                ExpiresInUTC = DateTime.UtcNow.AddMinutes(22),
                IssuedInUTC = DateTime.UtcNow,
                PasswordResetToken = Guid.NewGuid(),
                UserId = user.Id
            };
            _userRepo.InsertPasswordResetInfo(passwordResetInfo);

            var resetPasswordURL = $"{Config.EmailRegistrationBaseUrl}/ResetPassword/{passwordResetInfo.PasswordResetToken}/{user.Id}";
            var res = new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/SendResetPasswordEmail?code={Config.SendResetPasswordEmail}",
     new StringContent(JsonConvert.SerializeObject(
     new EmailDetails() { ToEmail = user.Email, URL = resetPasswordURL, FirstName = user.FirstName }), Encoding.UTF8, "application/json")).Result;
        }
        public void ForgotuserName(string emailAddress)
        {
            var user = _userRepo.GetByEmail(emailAddress);
            if (user == null) return;

            var resetPasswordURL = string.Empty;
            var res = new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/SendUserNameEmail?code={Config.SendUserNameEmail}",
     new StringContent(JsonConvert.SerializeObject(
     new UserNameForgotEmail() { ToEmail = user.Email, URL = resetPasswordURL, FirstName = user.FirstName, UserName = user.UserName }), Encoding.UTF8, "application/json")).Result;
        }
        public void OneTimeRegisterHeadCoach(string organizationName, string password, string userName, Guid emailValToken, int userId)
        {
            var salt = Guid.NewGuid();//todo: i dont know why we need this. further investigate please
            _userRepo.OneTimeRegisterHeadCoach(organizationName, Crypter.Blowfish.Crypt(password), userName, emailValToken, userId);
        }

        public bool CheckIfUserNameAlreadyExists(string userName)
        {
            return _userRepo.Get(userName) != null;
        }
        /// <summary>
        /// Creates a new user, it what you as a developer still need to do is create the athlete/coach 
        /// </summary>
        /// <exception cref="MismatchingPasswordsException">Thrown when passwords dont match</exception>
        /// <exception cref="UserAlreadyExistsException">THrown when a user with the same userName exists</exception>
        /// <param name="newUser"></param>
        /// <returns></returns>
        public int CreateHeadCoach(User newUser, string confirmedPassword)
        {
            newUser.IsCoach = true;
            newUser.IsHeadCoach = true;
            if (confirmedPassword != newUser.Password) throw new MismatchingPasswordsException("Your Passwords Didnt Match, Please re-enter your passwords");
            if (_userRepo.Get(newUser.UserName).Id != 0) throw new UserAlreadyExistsException("This Username already exists, please enter a new username");// I do not want to overload the equality operators

            var salt = Guid.NewGuid();//todo: i dont know why we need this. further investigate please
            string encryptedPassword = Crypter.Blowfish.Crypt(newUser.Password);
            newUser.Password = encryptedPassword;
            newUser.ImageContainerName = Guid.NewGuid().ToString();
            newUser.EmailValidationToken = Guid.NewGuid().ToString();
            newUser.Id = _userRepo.Create(newUser);

            _orgRepo.AssignRole(newUser.Id, OrganizationRoleEnum.Admin, newUser.OrganizationId, newUser.Id);

            _userRepo.DupeDemoDataToUser(newUser);
            //todo: make the email registration work
            //var registrationCompleteURL = $"http://api.strengthcoachpro.com/UserEmailVerification/{newUser.EmailValidationToken}";
            //var res = new HttpClient().PostAsync("https://azurefunctions20190226035451.azurewebsites.net/api/SendRegisterUserEmail?code=uHiJIypsAHeOVgiUw2mLjK8fYlcW7SbWPaguuD1fmUxO2DGod5CgbQ==",
            //new StringContent(JsonConvert.SerializeObject(
            //new EmailDetails() { ToEmail = newUser.Email , URL = registrationCompleteURL }), Encoding.UTF8, "application/json"));
            return newUser.Id;
        }

        public bool FinishUserRegistration(string emailGuid)
        {

            return true;
        }
        /// <summary>
        /// Returns tuple(string,bool,string,string) string is new guid, bool is if they are are a coach, string is firstname, string is email
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Tuple<Guid, bool, string, string> Login(string userName, string password)
        {
            var targetUser = _userRepo.Get(userName);
            var firstName = targetUser.FirstName;
            if (targetUser.IsDeleted)
            {
                throw new ApplicationException("User Has Been Deleted, If this is an Error please contact your coach");
            }
            if (targetUser.Id == 0)
            {
                throw new UserNotFoundException("Login Information Is Incorrect");
            }
            if (!Crypter.CheckPassword(password, targetUser.Password))
            {
                throw new FailedLoginException("Login Information Is Incorrect");
            }
            if (!targetUser.IsCoach)
            {
                firstName = _athleteRepo.GetAthleteByAthleteUserId(targetUser.Id).FirstName;
            }

            var newToken = _userTokenRepo.GetExistingTokenOrGenerateNewOne(targetUser.Id);
            return new Tuple<Guid, bool, string, string>(newToken, targetUser.IsCoach, firstName, targetUser.Email);
        }

        public BusinessObjects.Token.VerifyToken VerifyToken(Guid userToken)
        {
            var targetUser = _userRepo.Get(userToken);
            var weightRoomUser = _weightRoomRepo.Get(userToken);
            //an orgId of 0 means it cant be found. So we assume its a weightRoom account
            if (targetUser.OrganizationId == 0 && weightRoomUser.OrganizationId == 0) throw new ApplicationException("Invalid account");
            var org = _orgRepo.GetOrg(targetUser.OrganizationId == 0 ? weightRoomUser.OrganizationId : targetUser.OrganizationId);

            //athletes dont need any of this and it was causing errors 
            return new BusinessObjects.Token.VerifyToken()
            {
                IsUser = targetUser.Id != 0,
                IsWeightRoomView = weightRoomUser != null,
                IsOrganizationACustomer = org == null ? true : org.IsCustomer,
                IsHeadCoach = targetUser.IsHeadCoach,
                UserId = targetUser.Id,
                StripeGuid = org == null ? string.Empty : org.StripeGuid,
                HasBadCreditCard = org.BadCreditCard || org.StripeFailedToProcess,
                HasSubscriptionEnded = org.SubscriptionEnded,
                IsCreditCardExpiring = org.CreditCardExpiring
            };
        }
        public void Logout(Guid token)
        {
            _userTokenRepo.DeleteOldTokens(token);
        }

        public Boolean CreateAndSaveStripeData(int organizationId, int subscriptionPlan, StripeCustomerData stripeCustomerData)
        {
            try
            {
                StripeConfiguration.ApiKey = StripeKey;
                var targetSubscription = _SubRepo.GetSubscription(subscriptionPlan);
                AddressOptions StripeAddress = new AddressOptions();
                StripeAddress.City = stripeCustomerData.City;
                StripeAddress.Line1 = stripeCustomerData.AddressLine1;
                StripeAddress.Line2 = stripeCustomerData.AddressLine2;
                StripeAddress.State = stripeCustomerData.State;
                StripeAddress.Country = stripeCustomerData.Country;
                StripeAddress.PostalCode = stripeCustomerData.Zip;
                Token token = CreateCard(stripeCustomerData);
                PaymentMethod paymentMethod = new PaymentMethod();
                var options = new CustomerCreateOptions();
                if (stripeCustomerData.Coupon != null && stripeCustomerData.Coupon.Length > 0)
                {
                    options = new CustomerCreateOptions
                    {
                        Email = stripeCustomerData.Email,
                        Name = stripeCustomerData.FirstName + " " + stripeCustomerData.LastName,
                        Phone = stripeCustomerData.Phone,
                        Address = StripeAddress,
                        Description = "Customer for " + stripeCustomerData.Email,
                        TrialEnd = DateTime.Now.AddDays(14),
                        Plan = targetSubscription.StripeSubscriptionGuid,
                        Coupon = stripeCustomerData.Coupon,
                        Source = token.Id
                    };
                }
                else
                {
                    options = new CustomerCreateOptions
                    {
                        Email = stripeCustomerData.Email,
                        Name = stripeCustomerData.FirstName + " " + stripeCustomerData.LastName,
                        Phone = stripeCustomerData.Phone,
                        Address = StripeAddress,
                        Description = "Customer for " + stripeCustomerData.Email,
                        TrialEnd = DateTime.Now.AddDays(14),
                        Plan = targetSubscription.StripeSubscriptionGuid,
                        Source = token.Id
                    };
                }
                var service = new CustomerService();
                var response = service.Create(options);
                _userRepo.UpdateStripeDetailsForOrganisation(organizationId, stripeCustomerData, subscriptionPlan, response.Id);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }



        public Token CreateCard(StripeCustomerData stripeCustomerData)
        {
            StripeConfiguration.ApiKey = StripeKey;
            var options = new TokenCreateOptions
            {
                Card = new CreditCardOptions
                {
                    Number = stripeCustomerData.CardNumber,
                    ExpYear = stripeCustomerData.ExpiryYear,
                    ExpMonth = stripeCustomerData.ExpiryMonth,
                    Cvc = "" + stripeCustomerData.CVC,
                    Name = stripeCustomerData.FirstName + " " + stripeCustomerData.LastName,
                }
            };
            var service = new TokenService();
            Token stripeToken = service.Create(options);
            return stripeToken;
        }

        public Session CreateStripeUpdateSession(int organizationId, int subscriptionPlan)
        {
            StripeConfiguration.ApiKey = StripeKey;
            var targetSubscription = _SubRepo.GetSubscription(subscriptionPlan);
            var targetOrg = _orgRepo.GetOrg(organizationId);
            var options = new SessionCreateOptions
            {
                SetupIntentData = new SessionSetupIntentDataOptions()
                {
                    Metadata = new Dictionary<string, string>() {
                        { "customer_id", targetSubscription.StripeSubscriptionGuid },
                        { "subscription_id", targetSubscription.StripeSubscriptionGuid}
                    }
                },
                Mode = "setup",
                SuccessUrl = EmailRegistrationBaseUrl + "/Home?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = EmailRegistrationBaseUrl + "/Login?cancelout=1&orgId=1",
                PaymentMethodTypes = new List<string> {
                "card",
                },
            };
            var service = new SessionService();
            Session session = service.Create(options);
            _orgMan.UpdateStripeSessionForOrganization(organizationId, session.Id, targetSubscription.Id);
            return session;
        }

        public Session CreateStripeUserSession(int organizationId, int subscriptionPlan)
        {
            StripeConfiguration.ApiKey = StripeKey;
            var targetSubscription = _SubRepo.GetSubscription(subscriptionPlan);


            var options = new SessionCreateOptions
            {
                SubscriptionData = new SessionSubscriptionDataOptions
                {
                    Items = new List<SessionSubscriptionDataItemOptions> {
                    new SessionSubscriptionDataItemOptions {
                    Plan = targetSubscription.StripeSubscriptionGuid,
                    },
                  },
                    TrialPeriodDays = 14
                },
                SuccessUrl = EmailRegistrationBaseUrl + "/Home?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = EmailRegistrationBaseUrl + $"/Home?cancelout=1&orgId={organizationId}",
                PaymentMethodTypes = new List<string> {
                "card",
                },
            };
            var service = new SessionService();
            Session session = service.Create(options);
            _orgMan.UpdateStripeSessionForOrganization(organizationId, session.Id, targetSubscription.Id);
            return session;
        }


        public Session CreateStripeUserSessionForUpdate(Guid guid)
        {
            User user = _userRepo.Get(guid);
            if (user == null) { return null; }
            string StripeGuid = _userRepo.GetStripeGuIdForOrganization(user.OrganizationId);
            StripeConfiguration.ApiKey = StripeKey;
            var customerService = new CustomerService();
            Customer customer = customerService.Get(StripeGuid);
            string subscription_id = "";
            if (customer.Subscriptions.Data.Count > 0)
            {
                subscription_id = customer.Subscriptions.Data[0].Id;
            }

            var options = new SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> {
                  "card",
            },
                Customer = StripeGuid,
                SetupIntentData = new SessionSetupIntentDataOptions
                {
                    Metadata = new Dictionary<String, String>(){
                        {"customer_id", StripeGuid},
                        {"subscription_id", subscription_id},
                    }
                },
                Mode = "setup",
                SuccessUrl = EmailRegistrationBaseUrl + "/Home?session_id={CHECKOUT_SESSION_ID}",
                CancelUrl = EmailRegistrationBaseUrl + $"/Home?cancelout=1&orgId={user.OrganizationId}",
            };
            var service = new SessionService();
            Session session = service.Create(options);
            return session;
        }

        public void UpdateVisitedCount(Guid token, int UpdateField)
        {
            var targetUser = _userRepo.Get(token);
            _userRepo.UpdateVisitedUserCount(Enum.GetName(typeof(Models.Enums.Visited), UpdateField), targetUser.Id);
        }

        public UserVisitedStatus GetVisitedCount(Guid token)
        {
            var targetUser = _userRepo.Get(token);
            return _userRepo.GetVisitedStatusForUser(targetUser.Id);
        }

        public bool CancelSubscription(Guid token, string feedback = "")
        {
            var targetUser = _userRepo.Get(token);
            var targetOrg = _orgRepo.GetOrg(targetUser.OrganizationId);
            _orgRepo.MarkOrgsWithoutCustomers(new List<int>() { targetOrg.Id });

            if (!string.IsNullOrEmpty(feedback))
            {
                _adminRepo.AddFeedBack(targetUser.Id, feedback);

            }
            _orgRepo.AddSubscriptionAudit(targetUser, targetOrg.CurrentSubscriptionPlanId.HasValue ? targetOrg.CurrentSubscriptionPlanId.Value : -1, -1);
            var customerId = _userRepo.GetStripeGuIdForUser(targetUser.Id);
            var dataPackage = new StringContent(JsonConvert.SerializeObject(new DeleteInfo() { CustomerId = customerId }), Encoding.UTF8, "application/json");


            var res = new HttpClient().PostAsync($"{Config.AzureFunctionsBaseUrl}/DeleteSubscription?code={Config.DeleteStripeAccount}", dataPackage).Result;
            if (res.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                throw new Exception("Couldnt Cancel Subscription");
            }
        }

        public User GetUserDetails(Guid guid)
        {
            User user = _userRepo.Get(guid);
            user.Password = "";
            return user;
        }

        public User UpdateUserDetails(Guid guid, User user)
        {
            return _userRepo.Get(guid);
        }
    }
}
