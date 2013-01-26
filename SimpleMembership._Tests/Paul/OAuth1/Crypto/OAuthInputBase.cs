using System;
using System.Net.Http;

namespace SimpleMembership._Tests.Paul.OAuth1.Crypto
{
    public abstract class OAuthInputBase
    {
        public const string SIGNATURE_METHOD = OAuth.V1.SIGNATURE_METHOD;
        public const string VERSION = OAuth.V1.VERSION;

        private string _timestamp;
        private string _nonce;

        public Uri RequestUri { get; protected set; }
        public Creds Consumer { get; private set; }
        public HttpMethod HttpMethod { get; private set; }

        protected OAuthInputBase(Creds consumer, HttpMethod method, string url)
        {
            Consumer = consumer;
            HttpMethod = method;
            RequestUri = new Uri(url);
        }

        protected OAuthInputBase(Creds consumer, HttpMethod method)
        {
            Consumer = consumer;
            HttpMethod = method;
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

    public class RequestTokenInput : OAuthInputBase
    {
        public string Callback { get; private set; }

        public RequestTokenInput(Creds consumer, string callback = "oob")
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.REQUEST_TOKEN)
        {
            Callback = callback;
        }
    }


    public class AuthorizeInput : OAuthInputBase
    {
        public string Verifier { get; set; }

        public AuthorizeInput(Creds consumer, string token, string verifier)
            : base(consumer, HttpMethod.Get)
        {
            Verifier = verifier;

            var url = OAuth.V1.Routes.GetAuthorizeTokenRoute(token);
            RequestUri = new Uri(url);
        }
    }


    public class AccessTokenInput : OAuthInputBase
    {
        public string Token { get; set; }
        public string SessionHandle { get; set; }

        public AccessTokenInput(Creds consumer, string token, string sessionHandle)
            : base(consumer, HttpMethod.Post)
        {
            Token = token;
            SessionHandle = sessionHandle;
            var url = OAuth.V1.Routes.ACCESS_TOKEN;
            RequestUri = new Uri(url);
        }
    }
}