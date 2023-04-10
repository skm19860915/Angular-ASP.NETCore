using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Stripe;
using DAL.Repositories;
using System.Net.Http;
using System.Net;
using System.Linq;
namespace AzureFunctions
{
    public static class StripeWebHooks
    {
        [FunctionName("CustomerDeletion")]
        public static async Task<HttpResponseMessage> CustomerDeletion([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                var json = await new StreamReader(req.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ParseEvent(json);
                var orgRepo = new OrganizationRepo(Config.SqlConn);
                if (stripeEvent.Type == Events.CustomerDeleted)
                {
                    var customer = stripeEvent.Data.Object as Customer;
                    var targetOrg = orgRepo.GetOrg(customer.Id);
                    orgRepo.MarkOrgNoLongerCustomer(targetOrg.Id);
                }
                
            }
            catch (Exception)
            {
                NotifyErrors(""); 
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }


        [FunctionName("CardExpiring")]
        public static async Task<HttpResponseMessage> CardExpiring([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                var json = await new StreamReader(req.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ParseEvent(json);
                var orgRepo = new OrganizationRepo(Config.SqlConn);
                if (stripeEvent.Type == Events.CustomerSourceExpiring)
                {
                    var card = stripeEvent.Data.Object as Card;
                    var targetOrg = orgRepo.GetOrg(card.Customer.Id);
                    orgRepo.CreditCardExpiring(targetOrg.Id);
                }

            }
            catch (Exception)
            {
                NotifyErrors("");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [FunctionName("CardUpdated")]
        public static async Task<HttpResponseMessage> CardUpdated([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                var json = await new StreamReader(req.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ParseEvent(json);
                var orgRepo = new OrganizationRepo(Config.SqlConn);
                if (stripeEvent.Type == Events.CustomerSourceExpiring)
                {
                    var card = stripeEvent.Data.Object as Card;
                    var targetOrg = orgRepo.GetOrg(card.Customer.Id);
                    orgRepo.CreditCardUpdated(targetOrg.Id);
                }

            }
            catch (Exception)
            {
                NotifyErrors("");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [FunctionName("SubscriptionEnded")]
        public static async Task<HttpResponseMessage> SubscriptionEnded([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                var json = await new StreamReader(req.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ParseEvent(json);
                var orgRepo = new OrganizationRepo(Config.SqlConn);
                if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {
                    
                    var sub = stripeEvent.Data.Object as Subscription;
                    var targetOrg = orgRepo.GetOrg(sub.Customer.Id);
                    orgRepo.SubscriptionEnded(targetOrg.Id);
                }

            }
            catch (Exception)
            {
                NotifyErrors("");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [FunctionName("SubscriptionCreated")]
        public static async Task<HttpResponseMessage> SubscriptionCreated([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                var json = await new StreamReader(req.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ParseEvent(json);
                var orgRepo = new OrganizationRepo(Config.SqlConn);
                var subRepo = new SubscriptionRepo(Config.SqlConn);
                if (stripeEvent.Type == Events.CustomerSubscriptionDeleted)
                {

                    var sub = stripeEvent.Data.Object as Subscription;
                    var targetSub = subRepo.GetAllSubscriptions().Where(x => x.StripeSubscriptionGuid == sub.Id).FirstOrDefault();
                    var targetOrg = orgRepo.GetOrg(sub.Customer.Id);
                    orgRepo.SubscriptionStarted(targetOrg.Id,targetSub.Id);
                }

            }
            catch (Exception)
            {
                NotifyErrors("");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [FunctionName("BadCreditCard")]
        public static async Task<HttpResponseMessage> BadCreditCard([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                var json = await new StreamReader(req.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ParseEvent(json);
                var orgRepo = new OrganizationRepo(Config.SqlConn);
                if (stripeEvent.Type == Events.SourceFailed || stripeEvent.Type == Events.SourceCanceled || stripeEvent.Type == Events.CustomerSourceDeleted)
                {

                    var card = stripeEvent.Data.Object as Card;
                    var targetOrg = orgRepo.GetOrg(card.Customer.Id);
                    orgRepo.BadCreditCard(targetOrg.Id);
                }

            }
            catch (Exception)
            {
                NotifyErrors("");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [FunctionName("UpdateCreditCard")]
        public static async Task<HttpResponseMessage> UpdateCreditCard([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {
            try
            {
                var json = await new StreamReader(req.Body).ReadToEndAsync();
                var stripeEvent = EventUtility.ParseEvent(json);
                var orgRepo = new OrganizationRepo(Config.SqlConn);
                if (stripeEvent.Type == Events.SourceChargeable || stripeEvent.Type == Events.CustomerSourceCreated || stripeEvent.Type == Events.CustomerSourceUpdated)
                {

                    var card = stripeEvent.Data.Object as Card;
                    var targetOrg = orgRepo.GetOrg(card.Customer.Id);
                    orgRepo.CreditCardUpdated(targetOrg.Id);
                }

            }
            catch (Exception)
            {
                NotifyErrors("");
                return new HttpResponseMessage(HttpStatusCode.BadRequest);
            }
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private static void NotifyErrors(string message)
        {

        }
    }
}
