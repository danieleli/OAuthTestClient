#region

using System;
using System.Net.Http;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1
{
    /// <summary>
    /// Parameter Aggregator Pattern
    /// </summary>
    public abstract class OAuthParametersBase
    {
        private string _nonce;
        private string _timestamp;

        #region -- Public Properties --

        public const string SIGNATURE_METHOD = AuthParameterFactory.Values.SIGNATURE_METHOD;
        public const string VERSION = AuthParameterFactory.Values.VERSION;
        public Uri RequestUri { get; private set; }
        public Creds Consumer { get; private set; }
        public HttpMethod HttpMethod { get; private set; }
        public string Timestamp
        {
            get { return _timestamp ?? (_timestamp = OAuthUtils.GenerateTimeStamp()); }
        }
        public string Nonce
        {
            get { return _nonce ?? (_nonce = OAuthUtils.GenerateNonce()); }
        }

        #endregion -- Properties --

        #region -- Constructor --

        protected OAuthParametersBase(Creds consumer, HttpMethod method, string url)
        {
            ValidateInputs(consumer, method, url);

            Consumer = consumer;
            HttpMethod = method;
            RequestUri = new Uri(url);
        }

        #endregion -- Constructor --

        #region -- Validation --

        private void ValidateInputs(Creds consumer, HttpMethod method, string url)
        {
            NullCheck(consumer, "consumer");
            NullCheck(method, "httpMethod");
            NullCheck(url, "url");
        }

        protected void NullCheck(string s, string name)
        {
            if (string.IsNullOrWhiteSpace(s))
                throw new ArgumentNullException(name);
        }

        protected static void NullCheck(object o, string name)
        {
            if (o == null)
                throw new ArgumentNullException(name);
        }

        #endregion -- Validation --

    }


    public class RequestTokenParameters : OAuthParametersBase
    {
        public string Callback { get; private set; }

        public RequestTokenParameters(Creds consumer, string callback = "oob")
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.REQUEST_TOKEN)
        {
            Callback = callback;
        }
    }


    public class AccessTokenParameters : OAuthParametersBase
    {
        public string Verifier { get; set; }
        public string SessionHandle { get; set; }

        public AccessTokenParameters(Creds consumer, string verifier, string sessionHandle)
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.ACCESS_TOKEN)
        {
            NullCheck(verifier, "oauth_verifier");

            Verifier = verifier;
            SessionHandle = sessionHandle;
        }
    }



    //public class AuthorizeParameters : OAuthParametersBase
    //{
    //    public string Verifier { get; set; }

    //    public AuthorizeParameters(Creds consumer, string token, string verifier)
    //        : base(consumer, HttpMethod.Get, OAuth.V1.Routes.GetAuthorizeTokenRoute(token))
    //    {
    //        NullCheck(token, "oauth_token");
    //        NullCheck(verifier, "oauth_verifier");
    //        Verifier = verifier;
    //    }
    //}



    // List of know and used oauth parameters' names
}
