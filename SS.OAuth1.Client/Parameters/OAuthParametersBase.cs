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
        private readonly bool _includeVersion;

        #region -- Properties --

        private string _nonce;
        private string _timestamp;

        public const string SIGNATURE_METHOD = OAuth.V1.Values.SIGNATURE_METHOD;
        public const string VERSION = OAuth.V1.Values.VERSION;

        public Creds Consumer { get; protected set; }
        public HttpMethod HttpMethod { get; private set; }
        public Uri RequestUri { get; private set; }
        public string Authority { get; private set; }

        public virtual string Timestamp
        {
            get { return _timestamp ?? (_timestamp = OAuth.GenerateTimeStamp()); }
        }

        public virtual string Nonce
        {
            get { return _nonce ?? (_nonce = OAuth.GenerateNonce()); }
        }

        #endregion -- Properties --

        // Constructor
        protected OAuthParametersBase(Creds consumer, HttpMethod method, string url, string authority = null, bool includeVersion = true)
        {
            _includeVersion = includeVersion;
            ValidateInputs(consumer, method, url);

            this.Consumer = consumer;
            this.HttpMethod = method;
            this.Authority = authority;
            this.RequestUri = new Uri(url);
        }


        
        // Public Methods
        
        public abstract NameValueCollection GetOAuthParams();

        public virtual string GetOAuthHeader()
        {
            return GetOAuthHeader(null);
        }

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

        public string GetSignatureBase(NameValueCollection httpContent)
        {
            var method = this.HttpMethod.ToString().ToUpper();
            var baseUri = this.RequestUri.GetBaseStringUri().UrlEncodeForOAuth();
            var paramz = this.GetAllRequestParameters(httpContent).Normalize().UrlEncodeForOAuth();

            var rtn = string.Format("{0}&{1}&{2}", method, baseUri, paramz);

            return rtn;
        }



        // Protected Methods

        protected string GetOAuthHeader(string callback)
        {
            var requestToken = new Creds(null, null);
            return this.GetOAuthHeader(requestToken, callback);
        }

        protected string GetOAuthHeader(Creds requestToken, string callback = null, string verifier = null)
        {
            requestToken = requestToken ?? new Creds(null, null);

            var oauthParamDictionary = this.GetOAuthParams();

            var sig = CreateSignature(this, requestToken, callback, verifier);

            oauthParamDictionary.Add(OAuth.V1.Keys.SIGNATURE, sig);

            return oauthParamDictionary.Stringify();
        }

        protected NameValueCollection GetOAuthParamsCore()
        {
            var d = new NameValueCollection();
            d.AddIfNotNullOrEmpty(OAuth.V1.Keys.NONCE, this.Nonce);
            d.AddIfNotNullOrEmpty(OAuth.V1.Keys.TIMESTAMP, this.Timestamp);
            d.AddIfNotNullOrEmpty(OAuth.V1.Keys.CONSUMER_KEY, this.Consumer.Key);
            d.AddIfNotNullOrEmpty(OAuth.V1.Keys.SIGNATURE_METHOD, OAuth.V1.Values.SIGNATURE_METHOD);

            if (_includeVersion)
            {
                d.AddIfNotNullOrEmpty(OAuth.V1.Keys.VERSION, OAuth.V1.Values.VERSION);
            }

            return d;
        }                           



        // Private Method

        [Obsolete]
        private static string CreateSignature(OAuthParametersBase paramz, Creds requestToken, string callback = null, string verifier = null)
        {
            string requestTokenKey = null;
            string requestTokenSecret = null;

            if (requestToken != null)
            {
                requestTokenKey = requestToken.Key;
                requestTokenSecret = requestToken.Secret;
            }

            var signature = Signature.GetOAuth1ASignature(paramz.RequestUri,
                                                            paramz.HttpMethod,
                                                            paramz.Consumer.Key,
                                                            paramz.Consumer.Secret,
                                                            requestTokenKey,
                                                            requestTokenSecret,
                                                            paramz.Timestamp,
                                                            paramz.Nonce,
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