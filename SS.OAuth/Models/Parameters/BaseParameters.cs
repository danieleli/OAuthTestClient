using System.Collections.Specialized;
using SS.OAuth.Extensions;
using SS.OAuth.Misc;

namespace SS.OAuth.Models.Parameters
{
    public class BaseParameters
    {
        #region -- Properties --

        private readonly bool _includeVersion;
        private string _nonce;
        private string _timestamp;
        
        public const string SIGNATURE_METHOD = OAuth.V1.Values.SIGNATURE_METHOD;
        public const string VERSION = OAuth.V1.Values.VERSION;

        public Creds Consumer { get; protected set; }
        public string Realm { get; protected set; }

        public virtual string Timestamp
        {
            get { return _timestamp ?? (_timestamp = Utils.GenerateTimeStamp()); }
        }

        public virtual string Nonce
        {
            get { return _nonce ?? (_nonce = Utils.GenerateNonce()); }
        }

        #endregion -- Properties --

        // Constructor
        protected BaseParameters(Creds consumer, string realm = null, bool includeVersion = true)
        {
            _includeVersion = includeVersion;
            this.Consumer = consumer;
            Realm = realm;
        }

        public virtual NameValueCollection GetOAuthParams()
        {
            var collection = new NameValueCollection();
            collection.AddIfNotNullOrEmpty(OAuth.V1.Keys.NONCE, this.Nonce);
            collection.AddIfNotNullOrEmpty(OAuth.V1.Keys.TIMESTAMP, this.Timestamp);
            collection.AddIfNotNullOrEmpty(OAuth.V1.Keys.REALM, this.Realm);
            collection.AddIfNotNullOrEmpty(OAuth.V1.Keys.CONSUMER_KEY, this.Consumer.Key);
            collection.AddIfNotNullOrEmpty(OAuth.V1.Keys.SIGNATURE_METHOD, OAuth.V1.Values.SIGNATURE_METHOD);

            if (_includeVersion)
            {
                collection.AddIfNotNullOrEmpty(OAuth.V1.Keys.VERSION, OAuth.V1.Values.VERSION);
            }

            return collection;
        }

        public virtual string GetSignatureKey()
        {
            return this.Consumer.Secret;
        }

        public virtual string GetOAuthHeader(string signature)
        {
            var col = new NameValueCollection();
            var oauthParams = GetOAuthParams();

            col.Add(oauthParams);
            col.Add(V1.Keys.SIGNATURE, signature);

            return "OAuth " + col.Stringify();

        }
    }

    public static class Extensions
    {
        public static void AddIfNotNullOrEmpty(this NameValueCollection collection, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                collection.Add(key, value);
            }
        }
    }
}
