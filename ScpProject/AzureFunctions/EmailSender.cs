using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.IO;
using Newtonsoft.Json;


namespace AzureFunctions
{
    //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
    public static class EmailSender
    {
        public static SendGridClient Client => new SendGridClient(Config.EmailAPIKey);
        public static EmailAddress FromEmail => new EmailAddress("noreply@strengthcoachpro.com", "Strength Coach Pro");
        [FunctionName("SendCreateAthleteEmail")]
        public static async Task<HttpResponseMessage> Run([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            EmailDetails data = JsonConvert.DeserializeObject<EmailDetails>(requestBody);
            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.IO.Path.Combine(context.FunctionAppDirectory, "NewAthlete.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{AthleteFirstName}", data.FirstName).Replace("{RegistrationURL}", data.URL).Replace("{Organization}", data.OrganizationName);
            var subject = "Complete Athlete Registration";
            var to = new EmailAddress(data.ToEmail, string.Empty);
            var plainTextContent = $"Your Coach has invited you to join Strength Coach Pro, the premiere workout management software. Please finish your registration at the following url {data.URL}";

            var msg = MailHelper.CreateSingleEmail(FromEmail, to, subject, plainTextContent, body);
            msg.HtmlContent = body;
            var response = await Client.SendEmailAsync(msg);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [FunctionName("AlertSalesToNewUserRegistration")]
        public static async Task<HttpResponseMessage> AlertSalesToNewUserRegistration([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            NewUserEmailAlert data = JsonConvert.DeserializeObject<NewUserEmailAlert>(requestBody);
            var body = $@"New User Registered FirstName: {data.FirstName} , LastName: {data.LastName}, Email: {data.Email}, Phone: {data.PhoneNumber}";
            var msg = MailHelper.CreateSingleEmail(FromEmail, new EmailAddress("steve@strengthcoachpro.com"), "New User Registration For Strength Coach Pro", body, body);
            var response = await Client.SendEmailAsync(msg);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [FunctionName("AssistantCoachEmailRegistration")]
        public static async Task<HttpResponseMessage> AssistantCoachEmailRegistration([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            EmailDetails data = JsonConvert.DeserializeObject<EmailDetails>(requestBody);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.IO.Path.Combine(context.FunctionAppDirectory, "NewAssistantCoachEmail.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{AssistantCoachFirstName}", data.FirstName).Replace("{RegistrationURL}", data.URL).Replace("{Organization}", data.OrganizationName);
            var subject = "Complete Coach Registration";
            var to = new EmailAddress(data.ToEmail, string.Empty);
            var plainTextContent = $"Your Coach has invited you to join Strength Coach Pro, the premiere workout management software. Please finish your registration at the following url {data.URL}";
            var msg = MailHelper.CreateSingleEmail(FromEmail, to, subject, plainTextContent, body);
            msg.HtmlContent = body;
            var response = await Client.SendEmailAsync(msg);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [FunctionName("SendRegisterUserEmail")]
        public static async Task<HttpResponseMessage> RegisterEmail([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            EmailDetails data = JsonConvert.DeserializeObject<EmailDetails>(requestBody);

            var subject = "Complete User Registration";
            var to = new EmailAddress(data.ToEmail, string.Empty);
            var plainTextContent = $"Congratulations, on choosing the premier workout application. We at Strengh Coach Pro are excited to take this journey with you!. To finish your registration please go to the following url. {data.URL}";
            var htmlContent = $"Congratulations, on choosing the premier workout application. We at <strong>Strengh Coach Pro</strong> are excited to take this journey with you! To finish your registration please go to the following url. {data.URL}";
            var msg = MailHelper.CreateSingleEmail(FromEmail, to, subject, plainTextContent, htmlContent);
            var response = await Client.SendEmailAsync(msg);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [FunctionName("SendUserNameEmail")]
        public static async Task<HttpResponseMessage> UserName([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            UserNameForgotEmail data = JsonConvert.DeserializeObject<UserNameForgotEmail>(requestBody);

            var subject = "Lost User Name Email";
            var to = new EmailAddress(data.ToEmail, string.Empty);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.IO.Path.Combine(context.FunctionAppDirectory, "ForgotUserName.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{FirstName}", data.FirstName).Replace("{Username}", data.UserName);
            var plainTextContent = $"We are sorry that your user name has been lost, your user name is { data.UserName}";
            var msg = MailHelper.CreateSingleEmail(FromEmail, to, subject, plainTextContent, body);
            var response = await Client.SendEmailAsync(msg);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
        [FunctionName("SendResetPasswordEmail")]
        public static async Task<HttpResponseMessage> SendResetPasswordEmail([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req, ExecutionContext context)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            EmailDetails data = JsonConvert.DeserializeObject<EmailDetails>(requestBody);

            var subject = "Password Reset";
            var to = new EmailAddress(data.ToEmail, string.Empty);

            string body = string.Empty;
            using (StreamReader reader = new StreamReader(System.IO.Path.Combine(context.FunctionAppDirectory, "PasswordReset.html")))
            {
                body = reader.ReadToEnd();
            }
            body = body.Replace("{FirstName}", data.FirstName).Replace("{Url}", data.URL);
            var plainTextContent = $"We are sorry that your use password no longer works, to reset your password follow this link { data.URL}. This URL will expire in 20 minutes";
            var msg = MailHelper.CreateSingleEmail(FromEmail, to, subject, plainTextContent, body);
            var response = await Client.SendEmailAsync(msg);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        [FunctionName("StripeWebHooksError")]
        public static async Task<HttpResponseMessage> StripeWebHooksError([HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)]HttpRequest req, ExecutionContext context)
        {
            string requestBody = new StreamReader(req.Body).ReadToEnd();
            StripErrorEmail data = JsonConvert.DeserializeObject<StripErrorEmail>(requestBody);
            var subject = "Stripe Errors";
            var to = new EmailAddress("george@strengthcoachpro.com", string.Empty);
            var plainTextContent = $"stripe customerId; {data.StripeCustomer}    error:message {data.ErrorMessage} occured on {data.OccuredTime}";
            var msg = MailHelper.CreateSingleEmail(FromEmail, to, subject, plainTextContent, plainTextContent);
            var response = await Client.SendEmailAsync(msg);
            return new HttpResponseMessage(HttpStatusCode.OK);
        }
    }
    public class StripErrorEmail
    {
        public string StripeCustomer { get; set; }
        public string ErrorMessage { get; set; }
        public System.DateTime OccuredTime { get; set; }
    }
    public class NewUserEmailAlert
    {
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
    }
    public class EmailDetails
    {
        public string ToEmail { get; set; }
        public string URL { get; set; }
        public string FirstName { get; set; }
        public string OrganizationName { get; set; }
    }
    public class UserNameForgotEmail
    {
        public string ToEmail { get; set; }
        public string URL { get; set; }
        public string FirstName { get; set; }
        public string UserName { get; set; }
    }

}

