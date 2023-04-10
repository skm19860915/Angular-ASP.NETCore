using Stripe;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using BL;
using System.Web.Configuration;
using System.Configuration;
using Stripe.BillingPortal;
using System.Collections.Generic;
using DAL.Repositories;

namespace Controllers.Controllers
{
    [RoutePrefix("api/stripe")]
    public class StripeController : ApiController
    {
        public IOrganizationManager _orgMan { get; set; }
        private IUserManager _userManager { get; set; }
        public StripeController(IOrganizationManager orgMan, IUserManager userMan)
        {
            _orgMan = orgMan;
            _userManager = userMan;
        }

        [HttpGet, Route("GetPortalURL")]
        public string GetPortal()
        {
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);

           var user = _userManager.GetUserDetails(userGuid);

            if (!user.IsHeadCoach) throw new ApplicationException("User Isnt Head Coach");

            var org = _orgMan.GetOrg(user.OrganizationId, false);
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeKey"];

            var options = new SessionCreateOptions
            {
                Customer = org.Org.StripeGuid,
                ReturnUrl = ConfigurationManager.AppSettings["EmailregistrationBaseUrl"]
            };
            var service = new SessionService();
            var session = service.Create(options);
            return session.Url;
        }
        [HttpPost, Route("CustomerSubscriptionUpdated")]
        public async Task<IHttpActionResult> CustomerSubscriptionUpdated()
        {
            var json = await Request.Content.ReadAsStringAsync();
           
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                if (stripeEvent.Type == Events.CustomerSubscriptionUpdated)
                {
                    var targetCustomer = stripeEvent.Data.Object as Subscription;

                    if (!targetCustomer.CancelAt.HasValue)
                    {
                        _orgMan.ResetSubscription(targetCustomer.Id);
                    }
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }

        }

        [HttpPost, Route("ResetCreditCard")]
        public async Task<IHttpActionResult> ResetCreditCard()
        {
            var json = await Request.Content.ReadAsStringAsync();
         
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                var targetCustomer = stripeEvent.Data.Object as Customer;
                var targetOrg = _orgMan.GetOrg(targetCustomer.Id);

                if (stripeEvent.Type == Events.CustomerUpdated)
                {
                    if (targetCustomer.InvoiceSettings.DefaultPaymentMethodId != targetOrg.CurrentPaymentMethod)
                    {
                        _orgMan.ResetCardInfo(targetCustomer.Id, targetCustomer.InvoiceSettings.DefaultPaymentMethodId);
                    }
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

        [HttpPost, Route("DeleteCustomer")]
        public async Task<IHttpActionResult> DeleteCustomer()
        {
            var json = await Request.Content.ReadAsStringAsync();
           
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                if (stripeEvent.Type == Events.CustomerDeleted)
                {
                    var targetCustomer = stripeEvent.Data.Object as Customer;
                    _orgMan.CancelSubscription(targetCustomer.Id);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }

        }
        [HttpPost, Route("PaymentFailed")]
        public async Task<IHttpActionResult> PaymentFailed()
        {
            var json = await Request.Content.ReadAsStringAsync();
           
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);
                if (stripeEvent.Type == Events.PaymentIntentPaymentFailed)
                {
                    var targetCustomer = stripeEvent.Data.Object as PaymentIntent;
                    _orgMan.MarkStripeFailedToProcess(targetCustomer.Id);
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }

        }

        // GET api/<controller>
        [HttpPost, Route("Index")]
        public async Task<IHttpActionResult> Index()
        {
            var json = await Request.Content.ReadAsStringAsync();
            try
            {
                var stripeEvent = EventUtility.ParseEvent(json);

                // Handle the event
                if (stripeEvent.Type == Events.ChargeFailed)
                {
                    var chargeIntent = stripeEvent.Data.Object as Charge;
                    _orgMan.MarkStripeFailedToProcess(chargeIntent.CustomerId);
                    // Then define and call a method to handle the successful payment intent.
                    // handlePaymentIntentSucceeded(paymentIntent);
                }
                else if (stripeEvent.Type == Events.CustomerSourceExpiring)
                {
                    var cardExpiring = stripeEvent.Data.Object as Card;
                    _orgMan.MarkCardExpiring(cardExpiring.CustomerId);
                }
                else if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                    var cancelSub = stripeEvent.Data.Object as Subscription;
                    _orgMan.CancelSubscription(cancelSub.CustomerId);
                    //todo: set up email to steve.
                }
                // else if (stripeEvent.Type == Events.Custom)
                //  else if (stripeEvent.Type == Events.CustomerSourceUpdated)
                // ... handle other event types
                else
                {
                    // Unexpected event type
                    return BadRequest();
                }
                return Ok();
            }
            catch (StripeException e)
            {
                return BadRequest();
            }
        }

        [HttpPost, Route("CreateCustomer")]
        public string CreateCustomer([FromBody] CreateCustomerAndCharge clientInfo)
        {
            //for some reason the front end calls this code once, but angular framework executes the call twice. 
            //putting a cheap fix in now because I cant figure out why angular calls this  method twice.
            
            var targetOrg = _orgMan.GetOrg(clientInfo.orgId).Org;

            if (targetOrg != null && !string.IsNullOrEmpty(targetOrg.StripeGuid)) return "Success";
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeKey"];
            var options = new CustomerCreateOptions
            {
                Address = new AddressOptions()
                {
                    Line1 = clientInfo.Addr1,
                    City = clientInfo.City,
                    Country = clientInfo.Country,
                    PostalCode = clientInfo.PostalCode,
                    State = clientInfo.State
                },
                Email = clientInfo.Email,
                Name = clientInfo.FirstName + ' ' + clientInfo.LastName,
                Phone = clientInfo.Phone
            };
            var service = new CustomerService();
            var ret = service.Create(options);
            AddCreditCardToCustomer(clientInfo, ret.Id);
            AddSubscriptionToCustomer(clientInfo, ret.Id);

            _orgMan.UpdateStripeForOrganization(ret.Id, clientInfo.orgId, clientInfo.planId);
            return "Success";
        }

        [HttpPost, Route("FixBadCustomerByRecreating")]
        public string FixBadCustomerByRecreating([FromBody] CreateCustomerAndCharge clientInfo)
        {
            //for some reason the front end calls this code once, but angular framework executes the call twice. 
            //putting a cheap fix in now because I cant figure out why angular calls this  method twice.
            var targetOrg = _orgMan.GetOrg(clientInfo.orgId).Org;
            var userGuid = Guid.Parse(Request.Headers.GetCookies().FirstOrDefault().Cookies.FirstOrDefault(x => x.Name == "userToken").Value);

            var targetUser = _userManager.GetUserDetails(userGuid);

            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeKey"];
            var options = new CustomerCreateOptions
            {
                Address = new AddressOptions()
                {
                    Line1 = clientInfo.Addr1,
                    City = clientInfo.City,
                    Country = clientInfo.Country,
                    PostalCode = clientInfo.PostalCode,
                    State = clientInfo.State
                },
                Email = targetUser.Email,
                Name = targetUser.FirstName + ' ' + targetUser.LastName
            };
            var service = new CustomerService();
            var ret = service.Create(options);
            AddCreditCardToCustomer(clientInfo, ret.Id);
            AddSubscriptionToCustomer(clientInfo, ret.Id);

            _orgMan.UpdateStripeForOrganization(ret.Id, clientInfo.orgId, clientInfo.planId);
            return "Success";
        }

        public void AddCreditCardToCustomer(CreateCustomerAndCharge info, string CustomerId)
        {
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeKey"];

            var options = new TokenCreateOptions
            {
                Card = new CreditCardOptions
                {
                    Number = info.CCNum.Replace(" ", ""),
                    ExpMonth = info.expirationMonth,
                    ExpYear = info.expirationYear,
                    Cvc = info.CVC.ToString(),
                    AddressLine1 = info.Addr1,
                    AddressCity = info.City,
                    AddressState = info.State,
                    AddressCountry = info.Country,
                    Name = info.FirstName + info.LastName
                },
            };
            var service = new TokenService();
            var ret = service.Create(options);


            var options2 = new CardCreateOptions
            {
                Source = ret.Id,
            };
            var service2 = new CardService();
            service2.Create(CustomerId, options2);
        }
        public void AddSubscriptionToCustomer(CreateCustomerAndCharge info, string CustomerId)
        {
            StripeConfiguration.ApiKey = ConfigurationManager.AppSettings["StripeKey"];

            var subscriptionId = new SubscriptionRepo(WebConfigurationManager.ConnectionStrings["scp"].ConnectionString).GetSubscription(info.planId);
            var options = new SubscriptionCreateOptions
            {
                Customer = CustomerId,
                Items = new List<SubscriptionItemOptions>
                {
                    new SubscriptionItemOptions
                    {
                        Price = subscriptionId.StripeSubscriptionGuid,
                    },
                },
                 // Coupon = "Zt6LjFgZ",
                TrialPeriodDays = 14
            };
            var service = new SubscriptionService();
            service.Create(options);
        }
    }

    public class CreateCustomerAndCharge
    {
        public string Addr1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string CCNum { get; set; }
        public int CVC { get; set; }
        public int expirationMonth { get; set; }
        public int expirationYear { get; set; }
        public int planId { get; set; }
        public int orgId { get; set; }

    }
}