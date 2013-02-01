using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using SS.OAuth1.Client.Extensions;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Messages;

namespace SS.OAuth1.Client.Parameters
{

    public interface IMessageParameters
    {
        HttpMethod HttpMethod { get; }
        Uri RequestUri { get; }
    }


    /// <summary>
    ///     Parameter Aggregator Pattern
    /// </summary>
    public abstract class OAuthParametersBase : IMessageParameters
    {
        #region -- Properties --

        private string _nonce;
        private string _timestamp;

        public const string SIGNATURE_METHOD = Values.SIGNATURE_METHOD;
        public const string VERSION = Values.VERSION;

        public Creds Consumer { get; protected set; }
        public HttpMethod HttpMethod { get; private set; }
        public Uri RequestUri { get; private set; }
        public string Authority { get; private set; }

        public string Timestamp
        {
            get { return _timestamp ?? (_timestamp = OAuth.GenerateTimeStamp()); }
        }

        public string Nonce
        {
            get { return _nonce ?? (_nonce = OAuth.GenerateNonce()); }
        }

        #endregion -- Properties --

        // Constructor
        protected OAuthParametersBase(Creds consumer, HttpMethod method, string url, string authority = null)
        {
            ValidateInputs(consumer, method, url);

            this.Consumer = consumer;
            this.HttpMethod = method;
            this.Authority = authority;
            this.RequestUri = new Uri(url);
        }

        public abstract NameValueCollection GetOAuthParams();

        public abstract string GetOAuthHeader();

        public NameValueCollection GetAllRequestParameters(NameValueCollection httpContent)
        {
            var rtnCollection = new NameValueCollection();
            if (httpContent != null)
            {
                rtnCollection.Add(httpContent);
            }

            var queryParams = this.RequestUri.ParseQueryString();
            var oauthParams = this.GetOAuthParams();

            rtnCollection.Add(queryParams);
            rtnCollection.Add(oauthParams);
            

            return rtnCollection;
        }

        protected NameValueCollection GetOAuthParamsCore()
        {
            var d = new NameValueCollection();
            d.AddIfNotNullOrEmpty(Keys.NONCE, this.Nonce);
            d.AddIfNotNullOrEmpty(Keys.TIMESTAMP, this.Timestamp);
            d.AddIfNotNullOrEmpty(Keys.CONSUMER_KEY, this.Consumer.Key);
            
            d.AddIfNotNullOrEmpty(Keys.VERSION, Values.VERSION);
            d.AddIfNotNullOrEmpty(Keys.SIGNATURE_METHOD, Values.SIGNATURE_METHOD);

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