#region

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Remoting;
using SimpleMembership._Tests.Paul.OAuth1.Crypto;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul
{
    public static class MsgHelper
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MsgHelper));

        [Obsolete]
        public static HttpRequestMessage CreateRequestMessage(string url, HttpMethod method)
        {
            var input = new RequestTokenInput(null);
            return CreateRequestMessage(input);
        }

        public static HttpRequestMessage CreateRequestMessage(RequestTokenInput input)
        {
            var msg = new HttpRequestMessage
                {
                    RequestUri = input.RequestUri,
                    Method = input.HttpMethod
                };

            AddMediaTypeHeader(msg);

            return msg;
        }

        private static void AddMediaTypeHeader(HttpRequestMessage msg)
        {
            var mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }

        public static HttpResponseMessage Send(HttpRequestMessage msg)
        {
            LOG.Debug("Message: \n" + msg + "\n");

            var httpClient = new HttpClient();
            var response = httpClient.SendAsync(msg).Result;

            ValidateResponse(response);

            return response;
        }

        private static void ValidateResponse(HttpResponseMessage response)
        {
            LOG.Debug("Response: \n" + response + "\n");
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