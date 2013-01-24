#region

using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Runtime.Remoting;
using log4net;

#endregion

namespace MXM.API.Test.Controllers
{
    public static class MsgHelper
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MsgHelper));

        public static HttpRequestMessage CreateRequestMessage(string url, HttpMethod verb)
        {
            var msg = new HttpRequestMessage
                {
                    RequestUri = new Uri(url),
                    Method = verb
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
            LOG.Debug("\n\n" + msg + "\n");

            var httpClient = new HttpClient();
            var response = httpClient.SendAsync(msg).Result;

            ValidateResponse(response);

            return response;
        }

        private static void ValidateResponse(HttpResponseMessage response)
        {
            var isAuthorized = (response.StatusCode != HttpStatusCode.Unauthorized);
            if (!isAuthorized) throw new UnauthorizedAccessException("Unauthorized");

            var isServerError = (response.StatusCode != HttpStatusCode.InternalServerError);
            if (!isServerError)
            {
                LOG.Error("Server Error: " + response.ReasonPhrase);
                throw new ServerException("InternalServerError: '" + response.ReasonPhrase +"'");
            }
        }
    }
}