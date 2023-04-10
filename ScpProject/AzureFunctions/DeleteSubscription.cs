using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Stripe;

namespace AzureFunctions
{
    public static class DeleteSubscription
    {
        static string ApiKey = Config.StripeAPIKey;
        [FunctionName("DeleteSubscription")]
        public static async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ILogger log)
        {

            string requestBody = new StreamReader(req.Body).ReadToEnd();
            DeleteInfo data = JsonConvert.DeserializeObject<DeleteInfo>(requestBody);
            log.LogInformation("Azure Function DeleteSubscription...");
            StripeConfiguration.ApiKey = ApiKey;

            var customerService = new CustomerService();
            var customer = customerService.Get(data.CustomerId);
            var customerSubId = customer.Subscriptions.Data[0].Id;

            var service = new SubscriptionService();
            var response = service.Cancel(customerSubId, new SubscriptionCancelOptions() { Prorate = true, InvoiceNow = true });

            return new OkObjectResult("Deleted Successfully");
        }
    }
    public class DeleteInfo
    {
        public string CustomerId { get; set; }
    }
}
