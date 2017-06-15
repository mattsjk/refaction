using log4net;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace refactor_me.MessageHandlers
{

    /// <summary>
    /// Handler responsible for intercepting all HTTP requests and ensuring its header
    /// contains an API key. In this class, we are achieving this by overriding the
    /// SendAsync method. This method looks for an API key (apikey) in the header of
    /// every HTTP request, and passes the request to the controller only if a valid
    /// API key is present in the request header.
    /// 
    /// This class has been implemented to provide a simple and very basic
    /// security model for this WebApiDemo application
    /// </summary>
    public class ApiKeyHandler : DelegatingHandler
    {
        // Set a default API Key
        private const string defaultApiKey = "LAN1HTAL-ukhp-es0j-nahc-AHTAMGNUSMAS";
        private static readonly ILog logger = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancelToken)
        {
            bool isValidAPIKey = false;
            IEnumerable<string> lsHeaders;

            //Validate that the api key exists
            var checkApiKeyExists = request.Headers.TryGetValues("apikey", out lsHeaders);
            if (checkApiKeyExists)
            {
                logger.Info("SendAsync() - API Key [" + lsHeaders.FirstOrDefault() + "] received in request.");

                if (lsHeaders.FirstOrDefault().Equals(defaultApiKey))
                {
                    isValidAPIKey = true;
                }
            }

            //If the key is not valid, return an error http status code.
            if (!isValidAPIKey)
            {
                logger.Warn("SendAsync() - Invalid API Key received.");
                return request.CreateResponse(HttpStatusCode.Forbidden, "Invalid API Key. Unauthorised access is not allowed.");
            }

            var response = await base.SendAsync(request, cancelToken);

            // Return the response back up the request chain
            return response;
        }
    }
}