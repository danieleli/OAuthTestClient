#region

using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using SimpleMembership._Tests.Paul.OAuth1;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul
{
    public static class MessageFactory
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (MessageFactory));

        [Obsolete]
        public static HttpRequestMessage CreateRequestMessage(string url, HttpMethod method)
        {
            var input = new RequestTokenParameters(null);
            return CreateRequestMessage(input);
        }

        public static HttpRequestMessage CreateRequestMessage(RequestTokenParameters parameters)
        {
            var msg = new HttpRequestMessage
                {
                    RequestUri = parameters.RequestUri,
                    Method = parameters.HttpMethod
                };

            AddMediaTypeHeader(msg);

            return msg;
        }

        private static void AddMediaTypeHeader(HttpRequestMessage msg)
        {
            var mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }

        
    }
}