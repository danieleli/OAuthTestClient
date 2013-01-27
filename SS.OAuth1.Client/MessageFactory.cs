#region

using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using SS.OAuth1.Client.Parameters;
using log4net;

#endregion

namespace SS.OAuth1.Client
{
    public static class MessageFactory
    {
        public static HttpRequestMessage CreateRequestMessage(MessageParameters parameters)
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