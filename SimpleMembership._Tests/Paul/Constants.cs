using System.Collections.Generic;

namespace PPS.API.Constants
{
    public static class Extensions
    {
        public static void AddIfNotNull(this IDictionary<string, string> dic, string key, string value)
        {
            if (value == null) return;
            dic.Add(key, value);
        }
    }
    public partial class OAuth
    {
        public partial class V1
        {
            public partial class Keys
            {
                //
                // List of know and used oauth parameters' names
                //    
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

            public static IDictionary<string, string> GetOAuthParams(string callback, string consumerKey, string nonce, string signature, string signatureMethod, string timestamp, string token, string tokenSecret, string verifier, string oauthVersion)
            {
                var d = new Dictionary<string, string>();

                d.AddIfNotNull(Keys.CALLBACK, callback);
                d.AddIfNotNull(Keys.CONSUMER_KEY, consumerKey);
                d.AddIfNotNull(Keys.NONCE, nonce);
                d.AddIfNotNull(Keys.SIGNATURE, signature);
                d.AddIfNotNull(Keys.SIGNATURE_METHOD, signatureMethod);
                d.AddIfNotNull(Keys.TIMESTAMP, timestamp);
                d.AddIfNotNull(Keys.TOKEN, token);
                d.AddIfNotNull(Keys.TOKEN_SECRET, tokenSecret);
                d.AddIfNotNull(Keys.VERIFIER, verifier);
                d.AddIfNotNull(Keys.VERSION, oauthVersion);

                return d;
            }
        }
    }
}