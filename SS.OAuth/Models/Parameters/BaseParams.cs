using System.Collections.Specialized;
using SS.OAuth.Extensions;
using SS.OAuth.Misc;

namespace SS.OAuth.Models.Parameters
{

    public abstract class BaseParams
    {
        #region -- Properties --

        private string _nonce;
        private string _timestamp;

        protected BaseParams( bool includeVersion = true, string realm = null )
        {
            IncludeVersion = includeVersion;
            Realm = realm;
        }

        protected Creds Consumer { get; set; }
        protected Creds RequestToken { get; set; }
        public string Realm { get; set; }
        private bool IncludeVersion { get; set; }

        public virtual string Timestamp
        {
            get { return _timestamp ?? (_timestamp = Utils.GenerateTimeStamp()); }
        }

        public virtual string Nonce
        {
            get { return _nonce ?? (_nonce = Utils.GenerateNonce()); }
        }

        #endregion -- Properties --

        public abstract NameValueCollection ToCollection();

        /// <summary>
        /// key - is set to the concatenated values of:
        /// 
        /// 1.  The client shared-secret, after being encoded (Section 3.6).
        ///
        /// 2.  An "&" character (ASCII code 38), which MUST be included
        /// even when either secret is empty.
        /// 
        /// 3.  The token shared-secret, after being encoded (Section 3.6). 
        /// </summary>
        public string GetSignatureKey()
        {
            var key = this.Consumer.Secret.UrlEncodeForOAuth() + "&";
            if (this.RequestToken != null)
            {
                key += this.RequestToken.Secret.UrlEncodeForOAuth();
            }
            return key;
        }

        protected NameValueCollection ToCollectionInternal()
        {
            var col = new NameValueCollection
                {
                    {OAuth.V1.Keys.NONCE, this.Nonce},
                    {OAuth.V1.Keys.TIMESTAMP, this.Timestamp},
                    {OAuth.V1.Keys.SIGNATURE_METHOD, OAuth.V1.Values.SIGNATURE_METHOD},
                    {OAuth.V1.Keys.CONSUMER_KEY, this.Consumer.Key}
                };

            if (this.IncludeVersion) { col.Add(OAuth.V1.Keys.VERSION, OAuth.V1.Values.VERSION); }
            if (this.RequestToken != null) { col.AddIfNotNullOrEmpty(OAuth.V1.Keys.TOKEN, this.RequestToken.Key); }

            return col;
        }
    }
}
