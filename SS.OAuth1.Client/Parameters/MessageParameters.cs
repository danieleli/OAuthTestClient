using System;
using System.Net.Http;

namespace SS.OAuth1.Client.Parameters
{
    public class MessageParameters
    {
        public HttpMethod HttpMethod { get; protected set; }
        public Uri RequestUri { get; protected set; }

        protected MessageParameters(){ }

        public MessageParameters(Uri requestUri, HttpMethod httpMethod)
        {
            RequestUri = requestUri;
            HttpMethod = httpMethod;
        }
    }
}