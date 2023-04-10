namespace SignalRFunctions
{
    public class Config
    {
        public static string EmailAPIKey { get { return "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"; } }
        public static string SqlConn { get { return @"Server = tcp:strengthconpro.database.windows.net, 1433; Initial Catalog =test_scp; Persist Security Info=False;User ID = ghp9170; Password=nap1trag!@|?@T@R; MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout = 30;"; } }
        public static string StripeAPIKey { get { return "sk_live_rFhSUZv1roBRH8BEAwFTTMWh004Qnqbu7z"; } }
        public static bool logEverything => false;
    }
}

