using log4net;
using System.Web.Http;

namespace refactor_me
{
    public static class WebApiConfig
    {

        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static void Register(HttpConfiguration config)
        {
            logger.Info("Register() - Removing the XML formatter and enabling JSON as default response content type.");
            // Web API configuration and services
            var formatters = GlobalConfiguration.Configuration.Formatters;
            formatters.Remove(formatters.XmlFormatter);
            formatters.JsonFormatter.Indent = true;

            logger.Info("Register() - Registering the API routes using Attribute Routing.");
            // Web API routes
            config.MapHttpAttributeRoutes();

            logger.Info("Register() - Enabling the API routes for conventional routing.");
            // Fallback plan - may be omitted depending on the requirements
            //Attribute routing can be combined with convention - based routing.
            //To define convention - based routes, call the MapHttpRoute method.
            config.Routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional }
            );
        }
    }
}
