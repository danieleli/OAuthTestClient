using System.Collections.Specialized;
using SS.OAuth.Extensions;
using SS.OAuth.Misc;

namespace SS.OAuth.Models.Parameters
{
    public class BaseParameters
    {
        #region -- Properties --

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
        protected BaseParameters(Creds consumer, string realm = null)
        {
            this.Consumer = consumer;
            Realm = realm;
        }

        public virtual string GetSignatureKey()
        {
            return this.Consumer.Secret;
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
