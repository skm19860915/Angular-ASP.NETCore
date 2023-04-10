using System.Configuration;

namespace BL
{
    public class Config
    {

        public static string EmailRegistrationBaseUrl => ConfigurationManager.AppSettings["EmailregistrationBaseUrl"];
        public static string AzureFunctionsBaseUrl => ConfigurationManager.AppSettings["AzureFunctionsBaseURL"];
        public static string AssistantCoachEmailCode => ConfigurationManager.AppSettings["AssistantCoachEmailCode"];
        public static string GeneratePDFCode => ConfigurationManager.AppSettings["GeneratePDFCode"];
        public static string ResizeImageCode => ConfigurationManager.AppSettings["ResizeImageCode"];
        public static string SendCreateAthleteEmail => ConfigurationManager.AppSettings["SendCreateAthleteEmail"];
        public static string SendRegisterEmail => ConfigurationManager.AppSettings["SendRegisterEmail"];
        public static string SendResetPasswordEmail => ConfigurationManager.AppSettings["SendResetPasswordEmail"];
        public static string GenerateAthletePDF => ConfigurationManager.AppSettings["GenerateAthletePDF"];
        public static string SendUserNameEmail => ConfigurationManager.AppSettings["SendUserNameEmail"];
        public static string UpgradeStripeAccount => ConfigurationManager.AppSettings["UpgradeStripeAccount"];
        public static string DeleteStripeAccount => ConfigurationManager.AppSettings["deleteSubscription"];
        public static string GenerateNewPdfCode => ConfigurationManager.AppSettings["GenerateNewPdfCode"];
        public static bool logEverything => ConfigurationManager.AppSettings["turnOnLogging"].ToLower() == "true";
        public static string SignalRBaseUrl => ConfigurationManager.AppSettings["signalREndPoint"];
        public static string AssignProgramSnapShots => ConfigurationManager.AppSettings["AssignProgramSnapShots"];
    }
}
