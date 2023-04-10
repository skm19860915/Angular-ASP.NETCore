using System;
using System.Web.Http;

namespace Controllers
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            //todo: add unhandeled error that logs and emails
            //todo: switch jwt tokens

            try
            {
                GlobalConfiguration.Configure(WebApiConfig.Register);
            }
            catch (Exception e)
            {
                var n = e;
            }
            GlobalConfiguration.Configuration.EnsureInitialized();
        }
    }
}
