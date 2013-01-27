#region

using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting;
using log4net;

#endregion

namespace SS.OAuth1.Client
{
    public static class MessageSender
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MessageSender));

        public static HttpResponseMessage Send(HttpRequestMessage msg)
        {
            LOG.Debug("Send Message: \n" + msg + "\n");

            var httpClient = new HttpClient();
            var response = httpClient.SendAsync(msg).Result;
            
            LOG.Debug("Message Response: \n" + response + "\n");
            ValidateResponse(response);

            return response;
        }

        private static void ValidateResponse(HttpResponseMessage response)
        {
            
            var isAuthorized = (response.StatusCode != HttpStatusCode.Unauthorized);
            if (!isAuthorized)
            {
                var msg = GetErrorDetails(response);
                throw new UnauthorizedAccessException(msg);
            }

            var isServerError = (response.StatusCode == HttpStatusCode.InternalServerError);
            if (isServerError)
            {
                LOG.Error("Server Error: " + response.ReasonPhrase);
                var msg = "Internal Server Error: '{0}' \nMessage: {1}\nDetails:{2}";
                var content = response.Content.ReadAsStringAsync().Result;
                content = Uri.UnescapeDataString(content);
                msg = string.Format(msg, response.ReasonPhrase, content, "det");
                throw new ServerException(msg);
            }
        }


        private static string GetErrorDetails(HttpResponseMessage response)
        {
            var values = response.Content.ReadAsFormDataAsync().Result;
            var errorCode = values["errorCode"];
            var message = values["errorCode"];
            var msg = string.Format("ErrorCode: {0}; Message: {1}", errorCode, message);

            return msg;
        }
    }
}