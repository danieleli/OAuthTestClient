using System;
using System.Collections.Specialized;
using System.Globalization;

namespace SS.OAuth.Models.Parameters
{
    public class BaseParameter
    {
        private static readonly Random _Random = new Random();

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
            get { return _timestamp ?? (_timestamp = GenerateTimeStamp()); }
        }

        public virtual string Nonce
        {
            get { return _nonce ?? (_nonce = GenerateNonce()); }
        }

        #endregion -- Properties --

        // Constructor
        protected BaseParameter(Creds consumer, string realm = null, bool includeVersion = true)
        {
            _includeVersion = includeVersion;
            this.Consumer = consumer;
            Realm = realm;
        }

        public static string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        public static string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return _Random.Next(123400, 9999999).ToString(CultureInfo.InvariantCulture);
        }

        protected NameValueCollection GetOAuthParamsCore()
        {
            var d = new NameValueCollection();
            d.AddIfNotNullOrEmpty(OAuth.V1.Keys.NONCE, this.Nonce);
            d.AddIfNotNullOrEmpty(OAuth.V1.Keys.TIMESTAMP, this.Timestamp);
            d.AddIfNotNullOrEmpty(OAuth.V1.Keys.REALM, this.Realm);
            d.AddIfNotNullOrEmpty(OAuth.V1.Keys.CONSUMER_KEY, this.Consumer.Key);
            d.AddIfNotNullOrEmpty(OAuth.V1.Keys.SIGNATURE_METHOD, OAuth.V1.Values.SIGNATURE_METHOD);

            if (_includeVersion)
            {
                d.AddIfNotNullOrEmpty(OAuth.V1.Keys.VERSION, OAuth.V1.Values.VERSION);
            }

            return d;
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
