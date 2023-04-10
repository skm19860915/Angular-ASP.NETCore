using System.Web.Http;
using System.Web.Http.Cors;

namespace Controllers
{
    public static class WebApiConfig
    {
        public static void Register(HttpConfiguration config)
        {
            // Web API configuration and services
            var localHostCors = new EnableCorsAttribute("*", headers: "*", methods: "*") { SupportsCredentials = true };
            config.EnableCors(localHostCors);



            // Web API routes
            config.MapHttpAttributeRoutes();

            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
