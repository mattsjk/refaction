using refactor_me.MessageHandlers;
using System.Web.Http;

namespace refactor_me
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            // Register the API KEY verification handler - [Optional] Default = OFF
            //GlobalConfiguration.Configuration.MessageHandlers.Add(new ApiKeyHandler());

            // Register the API enpoint routing
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
