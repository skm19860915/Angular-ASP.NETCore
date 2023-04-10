using System.Web.Configuration;

namespace Controllers
{
    public class Config
    {
        public static string ConnectionString => WebConfigurationManager.ConnectionStrings["scp"].ConnectionString ;
    }
}