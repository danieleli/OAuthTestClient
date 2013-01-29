using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Models;

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
        private string _nonce;
        private string _timestamp;

        #region -- Public Properties --

        public const string SIGNATURE_METHOD = Values.SIGNATURE_METHOD;
        public const string VERSION = Values.VERSION;
        
        public Creds Consumer { get; protected set; }

        public string Timestamp
        {
            get { return _timestamp ?? (_timestamp = OAuth.GenerateTimeStamp()); }
        }

        public string Nonce
        {
            get { return _nonce ?? (_nonce = OAuth.GenerateNonce()); }
        }

        public HttpMethod HttpMethod { get; private set; }
        public Uri RequestUri { get; private set; }

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

        public HttpRequestMessage CreateRequestMessage()
        {
            var msg = new HttpRequestMessage(this.HttpMethod, this.RequestUri);
            AddMediaFormatter(msg);
            AddAuthHeader(msg);

            return msg;
        }

        protected virtual void AddMediaFormatter(HttpRequestMessage msg)
        {
            var mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        }

        protected abstract void AddAuthHeader(HttpRequestMessage msg);

        
        protected SortedDictionary<string, string> GetOAuthParams(string consumerKey,
                                                              string nonce,
                                                              string signature,
                                                              string timestamp,
                                                              string callback = "",
                                                              string token = "",
                                                              string verifier = "",
                                                              string oauthVersion = Values.VERSION,
                                                              string signatureMethod = Values.SIGNATURE_METHOD)
        {
            var d = new SortedDictionary<string, string>();

            d.AddIfNotNullOrEmpty(Keys.CALLBACK, callback);
            d.AddIfNotNullOrEmpty(Keys.CONSUMER_KEY, consumerKey);
            d.AddIfNotNullOrEmpty(Keys.NONCE, nonce);
            d.AddIfNotNullOrEmpty(Keys.SIGNATURE, signature);
            d.AddIfNotNullOrEmpty(Keys.SIGNATURE_METHOD, signatureMethod);
            d.AddIfNotNullOrEmpty(Keys.TIMESTAMP, timestamp);
            d.AddIfNotNullOrEmpty(Keys.TOKEN, token);
            d.AddIfNotNullOrEmpty(Keys.VERIFIER, verifier);
            d.AddIfNotNullOrEmpty(Keys.VERSION, oauthVersion);

            return d;
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