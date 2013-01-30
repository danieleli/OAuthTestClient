using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Messages;

namespace SS.OAuth1.Client.Parameters
{

    public interface IMessageParameters
    {
        HttpMethod HttpMethod { get; }
        Uri RequestUri { get; }
    }


    public class Keys
    {
        public const string CALLBACK = "oauth_callback";
        public const string CONSUMER_KEY = "oauth_consumer_key";
        public const string NONCE = "oauth_nonce";
        public const string SIGNATURE = "oauth_signature";
        public const string SIGNATURE_METHOD = "oauth_signature_method";
        public const string TIMESTAMP = "oauth_timestamp";
        public const string TOKEN = "oauth_token";
        public const string TOKEN_SECRET = "oauth_token_secret";
        public const string VERIFIER = "oauth_verifier";
        public const string VERSION = "oauth_version";
    }

    public static class Values
    {
        public const string VERSION = "1.0";
        public const string SIGNATURE_METHOD = "HMAC-SHA1";
    }

    /// <summary>
    ///     Parameter Aggregator Pattern
    /// </summary>
    public abstract class OAuthParametersBase : IMessageParameters
    {
        #region -- Properties --

        private string _nonce;
        private string _timestamp;
        private OAuthParser _oAuthParser;
        
        public const string SIGNATURE_METHOD = Values.SIGNATURE_METHOD;
        public const string VERSION = Values.VERSION;

        public Creds Consumer { get; protected set; }
        public HttpMethod HttpMethod { get; private set; }
        public Uri RequestUri { get; private set; }

        public string Timestamp
        {
            get { return _timestamp ?? (_timestamp = OAuth.GenerateTimeStamp()); }
        }

        public string Nonce
        {
            get { return _nonce ?? (_nonce = OAuth.GenerateNonce()); }
        }

        public OAuthParser OAuthParser
        {
            get { return _oAuthParser ?? (_oAuthParser = new OAuthParser()); }
            set { _oAuthParser = value; }
        }

        #endregion -- Properties --

        #region -- Constructor --

        protected OAuthParametersBase(Creds consumer, HttpMethod method, string url)
        {
            ValidateInputs(consumer, method, url);

            this.Consumer = consumer;
            this.HttpMethod = method;
            this.RequestUri = new Uri(url);
        }

        #endregion -- Constructor --

        public abstract string GetOAuthHeader();

        //protected abstract SortedDictionary<string, string> GetOAuthParamsNoSignature();
        //protected abstract string GetOAuthSignature();

        protected SortedDictionary<string, string> GetOAuthParamsNoSignature(string callback = "", string token = "", string verifier = null)
        {
            var sortedDictionary = OAuthParser.GetOAuthParamsNoSignature(this, callback, token, verifier);

            return sortedDictionary;
        }

        protected string GetOAuth1ASignature(Creds requestToken, string callback = "", string verifier = null)
        {
            if (requestToken == null) requestToken = new Creds("", "");

            var signature = Signature.GetOAuth1ASignature(this.RequestUri,
                                                            this.HttpMethod,
                                                            this.Consumer.Key,
                                                            this.Consumer.Secret,
                                                            requestToken.Key,
                                                            requestToken.Secret,
                                                            this.Timestamp,
                                                            this.Nonce,
                                                            callback,
                                                            verifier);
            return signature;
        }

        #region -- Validation --

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

        private void ValidateInputs(Creds consumer, HttpMethod method, string url)
        {
            NullCheck(consumer, "consumer");
            NullCheck(method, "httpMethod");
            NullCheck(url, "url");
        }

        #endregion -- Validation --

    }
}