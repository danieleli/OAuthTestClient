#region

using System;
using System.Net;
using System.Net.Http;
using System.Runtime.Remoting;
using log4net;

#endregion

namespace SS.OAuth1.Client
{
    public class MessageSender
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MessageSender));
        private  HttpClient _httpClient;

        public  MessageSender()
        {
            _httpClient = new HttpClient();
        }


        public MessageSender(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public HttpResponseMessage Send(HttpRequestMessage msg)
        {
            LOG.Debug("Send Message: \n" + msg + "\n");

            var httpClient = _httpClient;
            var response = httpClient.SendAsync(msg).Result;
            
            LOG.Debug("Message Response: \n" + response + "\n");
            ValidateResponse(response);

            return response;
        }

        private void ValidateResponse(HttpResponseMessage response)
        {
            
            var isAuthorized = (response.StatusCode != HttpStatusCode.Unauthorized);
            if (!isAuthorized)
            {
                var ex = GetUnauthorizedException(response);
                throw ex;
            }

            var isServerError = (response.StatusCode == HttpStatusCode.InternalServerError);
            if (isServerError)
            {
                var ex = GetServerException(response);
                throw ex;
            }
        }

        private ServerException GetServerException(HttpResponseMessage response)
        {
            var msg = "Internal Server Error: '{0}' \nMessage: {1}\nDetails:{2}";
            var content = response.Content.ReadAsStringAsync().Result;
            content = Uri.UnescapeDataString(content);
            msg = string.Format(msg, response.ReasonPhrase, content, "det");
            var ex = new ServerException(msg);
            return ex;
        }


        private UnauthorizedAccessException GetUnauthorizedException(HttpResponseMessage response)
        {
            var values = response.Content.ReadAsFormDataAsync().Result;
            var errorCode = values["errorCode"];
            var message = values["message"];
            var msg = string.Format("ErrorCode: {0}; Message: {1}", errorCode, message);
            return new UnauthorizedAccessException(msg);
       
        }
    }
}