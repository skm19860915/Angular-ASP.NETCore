using Stripe;
using System.Linq;

namespace AzureFunctions
{
    public static class StripeBalanceChecker
    {
        static string ApiKey = Config.StripeAPIKey;

        /// <summary>
        /// StripeBalanceChecker checks customers with negative balance associated to the stripe account with given api key
        /// </summary>
        /// <param name="myTimer"></param>
        /// <param name="log"></param>
        //[FunctionName("StripeBalanceChecker")]
        //public static void Run([TimerTrigger("0 0 12 * * *", RunOnStartup = true)]TimerInfo myTimer, ILogger log)
        //{
        //    var orgRepo = new OrganizationRepo(Config.SqlConn);
        //    var allCustomers = orgRepo.GetAllOrgs();
        //    var stripeCustomers = GetAllStripeCustomers();

        //    var allStripeCustomersWithoutASubscription = stripeCustomers.Where(x => !x.Subscriptions.Data.Any()).ToList();

        //    var allStripeCustomersWithoutASubscriptionExceptFounders = allStripeCustomersWithoutASubscription.Select(x => x.Id)
        //        .Except(allCustomers.Where(x => x.CurrentSubscriptionPlanId != 6).Select(x => x.StripeGuid));
        //    //var customersthatArentInStripe = allCustomers.Select(x => x.StripeGuid).Except(stripeCustomers.Data.Select(y => y.Id));

        //    //var customerIdsThatDoentExistInStripe = allCustomers.Where(x => !customersthatArentInStripe.Contains(x.StripeGuid)).Select(x => x.Id).ToList();
        //    //customerIdsThatDoentExistInStripe.AddRange(allCustomers.Where(x => x.StripeGuid == null).Select(x => x.Id).ToList());
        //    //customerIdsThatDoentExistInStripe.AddRange(allCustomers.Where(x => x.StripeGuid == Guid.Empty.ToString()).Select(x => x.Id).ToList());
        //    //  orgRepo.MarkOrgsWithoutCustomers(customerIdsThatDoentExistInStripe);
        //    //log.LogInformation($"Executing StripeBalanceChecker at: {DateTime.Now}");
        //    //StripeConfiguration.ApiKey = ApiKey;
        //    //var service = new CustomerService();
        //    //try
        //    //{
        //    //    StripeList<Customer> customersWithNegativeBalance = new StripeList<Customer>();
        //    //    StripeList<Customer> customers = getAllCustomers(log);
        //    //    if (customers != null && customers.Data != null && customers.Data.Count > 0)
        //    //    {
        //    //        customersWithNegativeBalance.Data = customers.Data.FindAll(item => item.Balance < 0);
        //    //    }
        //    //    //Response test - To be removed after processing
        //    //    customersWithNegativeBalance.Data.ForEach(item =>
        //    //    {
        //    //        log.LogInformation("Balance for " + item.Email + " = " + item.Balance);
        //    //    });
        //    //}
        //    //catch (Exception e)
        //    //{
        //    //    log.LogError(e.Message);
        //    //}
        //}

        /// <summary>
        /// Gets all Customers in the stripe account with the given api key
        /// </summary>
        /// <param name="log">Pre-Initialized Logger</param>
        /// <returns>List of all customers</returns>
        public static StripeList<Customer> GetAllStripeCustomers()
        {
            var customers = new StripeList<Customer>() { Data = new System.Collections.Generic.List<Customer>() };
            StripeConfiguration.ApiKey = ApiKey;
            var service = new CustomerService();
            var options = new CustomerListOptions() { Limit = 100 };

            var result = service.List(options);
            while (result != null && result.Data.Any())
            {
                customers.Data.AddRange(result.Data);
                options = new CustomerListOptions() { Limit = 100, StartingAfter = customers.Data.Last().Id };
                result = service.List(options);
            }
            //TODO: Alert dev ops team, cannot get balances for customers

            return customers;
        }
    }
}
