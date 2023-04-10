using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Stripe;
using System.Collections.Generic;
using System.Net.Http;

namespace AzureFunctions
{
    public static class StripeUpgrade
    {

        [FunctionName("StripeUpgrade")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            StripeUpgradeInfo data = JsonConvert.DeserializeObject<StripeUpgradeInfo>(requestBody);
            try
            {
                Upgrade(data);
                return new HttpResponseMessage(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {

                return new HttpResponseMessage(System.Net.HttpStatusCode.InternalServerError);
            }
        }

        /// <summary>
        /// Fetches the current subscription with respect to the given customer id
        /// </summary>
        /// <param name="customerId">Customer whose subscription is to be retrieved</param>
        /// <param name="log"></param>
        /// <returns>current subscription with respect to the given customer id</returns>
        //private static StripeList<Subscription> getCurrentSubscriptionsForCustomer(string customerId, ILogger log)
        //{
        //    StripeList<Subscription> subscriptions = new StripeList<Subscription>();
        //    StripeConfiguration.ApiKey = Config.StripeAPIKey;
        //    var service = new CustomerService();
        //    var customer = service.Get(customerId);
        //    if (customer != null && customer.Subscriptions != null && customer.Subscriptions.Data != null)
        //    {
        //        subscriptions.Data = customer.Subscriptions.Data;
        //    }
        //    return subscriptions;
        //}

        /// <summary>
        /// Upgrades the given customer's plan with the new plan & cancels the old subscription
        /// </summary>
        /// <param name="customerId">Customer whose plan has to be upgraded</param>
        /// <param name="plan">New plan to subscribe</param>
        /// <param name="log"></param>
        /// <param name="oldSubscription">Old subscription that has to be cancelled</param>
        /// <returns>True if successful and false if not successful</returns>
        private static void Upgrade(StripeUpgradeInfo newInfo)
        {

            StripeConfiguration.ApiKey = Config.StripeAPIKey;

            var customerService = new CustomerService();
            var customer = customerService.Get(newInfo.CustomerStripeId);
            var customerSubId = customer.Subscriptions.Data[0].Id;
            var service = new SubscriptionService();
            var currentServiceSub = service.Get(customerSubId);


            //var optionsd = new SubscriptionItemListOptions
            //{
            //    Subscription = customerSubId,
            //};
            //var serviceItems = new SubscriptionItemService();
            //var st = serviceItems.List(optionsd);
            //var sbitem = st.Data[0].Id;


            var upgradeOptions =
                new SubscriptionUpdateOptions
                {
                    Items = new List<SubscriptionItemOptions>
                    {
                        new SubscriptionItemOptions()
                        {
                            Id=currentServiceSub.Items.Data[0].Id,
                            Plan = newInfo.NewPlanStripeGuid,
                        },

                    },
                    Prorate = true,
                    CancelAtPeriodEnd = false,
                };
           // service.Cancel(sbitem, new SubscriptionCancelOptions() { Prorate = true, InvoiceNow = true });
            service.Update(customerSubId, upgradeOptions);
            
        }

        /// <summary>
        /// Calculates the next plan based on existing plans in the system & current subscription by the selected customer
        /// </summary>
        /// <param name="customerId">Customer whose next plan has to be calculated</param>
        /// <param name="log"></param>
        /// <param name="subscription">Fetches out the current subsctiption</param>
        /// <returns></returns>
        //private static Plan getNextPlan(string customerId, ILogger log, out Subscription subscription)
        //{
        //    Subscription planWithSubscription = null;
        //    //Get all plans from stripe with respect to api key
        //    StripeList<Plan> allPlans = getAllPlansForAccount(log);
        //    //Get all older subsciptions by the selected customer
        //    StripeList<Subscription> currentSubscriptions = getCurrentSubscriptionsForCustomer(customerId, log);
        //    List<Plan> activePlans = new List<Plan>();
        //    //Segregate only active plans for the given customer if there is atleast 1 subscription
        //    if (currentSubscriptions != null && currentSubscriptions.Data.Count > 0)
        //    {
        //        currentSubscriptions.Data.ForEach(item =>
        //        {
        //            if (item.Plan.Active)
        //            {
        //                planWithSubscription = item;
        //                activePlans.Add(item.Plan);
        //            }
        //        });
        //        activePlans = activePlans.OrderByDescending(o => o.Amount).ToList();
        //    }
        //    else
        //    {
        //        subscription = planWithSubscription;
        //        //if no active subscription, return the primary one
        //        return allPlans.OrderBy(plan => plan.Amount).ToList()[0];
        //    }

        //    //Even with multiple subscriptions, there is no active plan, return the basic plan
        //    if (activePlans == null || activePlans.Count == 0)
        //    {
        //        subscription = planWithSubscription;
        //        return allPlans.OrderBy(plan => plan.Amount).ToList()[0];
        //    }

        //    //Fetch Applicable plans with respect to highest existing plan
        //    List<Plan> applicablePlans = allPlans.Where(o => o.Amount > activePlans[0].Amount).OrderBy(plan => plan.Amount).ToList();

        //    //Return applicable plan
        //    subscription = planWithSubscription;
        //    return (applicablePlans != null && applicablePlans.Count > 0) ? applicablePlans[0] : null;
        //}

        ///// <summary>
        ///// Fetches all the available plans for the given account
        ///// </summary>
        ///// <param name="log"></param>
        ///// <returns>List of all available Plans</returns>
        //private static StripeList<Plan> getAllPlansForAccount(ILogger log)
        //{
        //    StripeConfiguration.ApiKey = ApiKey;
        //    var service = new PlanService();
        //    var options = new PlanListOptions { Limit = 10 };
        //    var plans = service.List(options);
        //    return plans;
        //}
    }
    public class StripeUpgradeInfo
    {
        public string CustomerStripeId { get; set; }
        public string NewPlanStripeGuid { get; set; }
        public string OldPlanStripeGuid { get; set; }
    }
}
