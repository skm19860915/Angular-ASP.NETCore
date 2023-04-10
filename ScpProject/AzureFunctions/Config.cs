using System;

namespace AzureFunctions
{
    public class Config
    {
        public static string EmailAPIKey { get { return Environment.GetEnvironmentVariable("EmailAPIKey", EnvironmentVariableTarget.Process); } }
        public static string SqlConn { get { return Environment.GetEnvironmentVariable("SQLConn", EnvironmentVariableTarget.Process); } }
        public static string StripeAPIKey { get { return Environment.GetEnvironmentVariable("StripeAPIKey", EnvironmentVariableTarget.Process); } }
        public static bool logEverything { get { return bool.Parse(Environment.GetEnvironmentVariable("LogEverything", EnvironmentVariableTarget.Process)); } }
    }
}

