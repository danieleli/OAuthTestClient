using System;
using System.Net.Http;

namespace SimpleMembership._Tests.Paul.OAuth1.Crypto
{
    public class RequestTokenSignatureInput
    {
        public const string SIGNATURE_METHOD = OAuth.V1.SIGNATURE_METHOD;
        public const string VERSION = OAuth.V1.VERSION;

        private string _timestamp;
        private string _nonce;
        private Uri _uri;

        public Creds Consumer { get; private set; }
        public string Callback { get; private set; }

        public RequestTokenSignatureInput(Creds consumer, string callback = "oob")
        {
            Consumer = consumer;
            Callback = callback;
        }

        public Uri RequestUri
        {
            get { return _uri ?? (_uri = new Uri(OAuth.V1.Routes.ACCESS_TOKEN)); }
        }

        public HttpMethod HttpMethod
        {
            get { return HttpMethod.Post; }
        }

        public string Timestamp
        {
            get { return _timestamp ?? (_timestamp = OAuthUtils.GenerateTimeStamp()); }
        }

        public string Nonce
        {
            get { return _nonce ?? (_nonce = OAuthUtils.GenerateNonce()); }
        }

    }
}