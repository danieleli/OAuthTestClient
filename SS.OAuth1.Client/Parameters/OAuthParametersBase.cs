using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace SS.OAuth1.Client.Parameters
{
    /// <summary>
    ///     Parameter Aggregator Pattern
    /// </summary>
    public abstract class OAuthParametersBase : MessageParameters
    {
        private string _nonce;
        private string _timestamp;

        #region -- Public Properties --

        public const string SIGNATURE_METHOD = AuthParameterFactory.Values.SIGNATURE_METHOD;
        public const string VERSION = AuthParameterFactory.Values.VERSION;
        
        
        public Creds Consumer { get; protected set; }

        public string Timestamp
        {
            get { return _timestamp ?? (_timestamp = OAuthHelper.GenerateTimeStamp()); }
        }

        public string Nonce
        {
            get { return _nonce ?? (_nonce = OAuthHelper.GenerateNonce()); }
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

        public abstract string GetAuthHeader();

        public string Stringify(SortedDictionary<string, string> paramz)
        {
            var sb = new StringBuilder();
            var isFirstItem = true;
            foreach (var p in paramz)
            {
                if (!isFirstItem)
                {
                    sb.Append(",");
                }
                var key = Uri.EscapeUriString(p.Key);
                var value = Uri.EscapeUriString(p.Value);
                sb.Append(string.Format("{0}=\"{1}\"", key, value));
                isFirstItem = false;
            }

            return sb.ToString();
        }

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
}