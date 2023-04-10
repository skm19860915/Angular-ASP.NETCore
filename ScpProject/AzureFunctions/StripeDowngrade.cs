using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Stripe;
using System.Collections.Generic;
using System.Linq;

namespace AzureFunctions
{
    public static class StripeDowngrade
    {
        static string ApiKey = "sk_test_Rj3taw0PRWj7IK0slWGt175d00QgsFDX6C";
        [FunctionName("StripeDowngrade")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation($"Starting StripeDowngrade at {DateTime.Now}");
            Subscription subscription = null;
            Plan nextPlan = getNextPlan(req.Query["customerid"], log, out subscription);
            log.LogInformation($"Completing StripeDowngrade at {DateTime.Now}");
            return (nextPlan == null) ? (ActionResult)new OkObjectResult("You are already on the Lowest Plan") : downgrade(req.Query["customerid"], nextPlan, log, subscription) ? (ActionResult)new OkObjectResult("Successfully Downgraded") : (ActionResult)new OkObjectResult("Failed to Downgrade");
        }

        /// <summary>
        /// Fetches the current subscription with respect to the given customer id
        /// </summary>
        /// <param name="customerId">Customer whose subscription is to be retrieved</param>
        /// <param name="log"></param>
        /// <returns>current subscription with respect to the given customer id</returns>
        private static StripeList<Subscription> getCurrentSubscriptionsForCustomer(string customerId, ILogger log)
        {
            StripeList<Subscription> subscriptions = new StripeList<Subscription>();
            StripeConfiguration.ApiKey = ApiKey;
            var service = new CustomerService();
            var customer = service.Get(customerId);
            if (customer != null && customer.Subscriptions != null && customer.Subscriptions.Data != null)
            {
                subscriptions.Data = customer.Subscriptions.Data;
            }
            return subscriptions;
        }

        /// <summary>
        /// Downgrades the given customer's plan with the new plan & cancels the old subscription
        /// </summary>
        /// <param name="customerId">Customer whose plan has to be upgraded</param>
        /// <param name="plan">New plan to subscribe</param>
        /// <param name="log"></param>
        /// <param name="oldSubscription">Old subscription that has to be cancelled</param>
        /// <returns>True if successful and false if not successful</returns>
        private static Boolean downgrade(string customerId, Plan plan, ILogger log, Subscription oldSubscription)
        {
            return false;
            //try
            //{
            //    var items = new List<SubscriptionItemOption> { new SubscriptionItemOption { Plan = plan.Id } };
            //    var options = new SubscriptionCreateOptions { Customer = customerId, Items = items };
            //    var service = new SubscriptionService();
            //    Subscription subscription = service.Create(options);
            //    service.Cancel(oldSubscription.Id, null);
            //}
            //catch (Exception e)
            //{
            //    log.LogError(e.Message);
            //    return false;
            //}
            //return true;
        }

        /// <summary>
        /// Calculates the next plan based on existing plans in the system & current subscription by the selected customer
        /// </summary>
        /// <param name="customerId">Customer whose next plan has to be calculated</param>
        /// <param name="log"></param>
        /// <param name="subscription">Fetches out the current subsctiption</param>
        /// <returns></returns>
        private static Plan getNextPlan(string customerId, ILogger log, out Subscription subscription)
        {
            Subscription planWithSubscription = null;
            //Get all plans from stripe with respect to api key
            StripeList<Plan> allPlans = getAllPlansForAccount(log);
            //Get all older subsciptions by the selected customer
            StripeList<Subscription> currentSubscriptions = getCurrentSubscriptionsForCustomer(customerId, log);
            List<Plan> activePlans = new List<Plan>();
            //Segregate only active plans for the given customer if there is atleast 1 subscription
            if (currentSubscriptions != null && currentSubscriptions.Data.Count > 0)
            {
                currentSubscriptions.Data.ForEach(item =>
                {
                    if (item.Plan.Active)
                    {
                        planWithSubscription = item;
                        activePlans.Add(item.Plan);
                    }
                });
                activePlans = activePlans.OrderByDescending(o => o.Amount).ToList();
            }
            else
            {
                subscription = planWithSubscription;
                //if no active subscription, return the primary one
                return allPlans.OrderBy(plan => plan.Amount).ToList()[0];
            }

            //Even with multiple subscriptions, there is no active plan, return the basic plan
            if (activePlans == null || activePlans.Count == 0)
            {
                subscription = planWithSubscription;
                return allPlans.OrderBy(plan => plan.Amount).ToList()[0];
            }

            //Fetch Applicable plans with respect to highest existing plan
            List<Plan> applicablePlans = allPlans.Where(o => o.Amount < activePlans[0].Amount).OrderByDescending(plan => plan.Amount).ToList();

            //Return applicable plan
            subscription = planWithSubscription;
            return (applicablePlans != null && applicablePlans.Count > 0) ? applicablePlans[0] : null;
        }

        /// <summary>
        /// Fetches all the available plans for the given account
        /// </summary>
        /// <param name="log"></param>
        /// <returns>List of all available Plans</returns>
        private static StripeList<Plan> getAllPlansForAccount(ILogger log)
        {
            StripeConfiguration.ApiKey = ApiKey;
            var service = new PlanService();
            var options = new PlanListOptions { Limit = 10 };
            var plans = service.List(options);
            return plans;
        }
    }
}
