using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;

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
            get { return _timestamp ?? (_timestamp = OAuth.GenerateTimeStamp()); }
        }

        public string Nonce
        {
            get { return _nonce ?? (_nonce = OAuth.GenerateNonce()); }
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

        public HttpRequestMessage CreateRequestMessage()
        {
            var msg = new HttpRequestMessage
            {
                RequestUri = this.RequestUri,
                Method = this.HttpMethod
            };

            AddMediaTypeHeader(msg);

            return msg;
        }

        private void AddMediaTypeHeader(HttpRequestMessage msg)
        {
            var mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
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