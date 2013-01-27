#region

using System.Collections.Generic;

#endregion

namespace SS.OAuth1.Client.Parameters
{
    public static class Extensions
    {
        public static void AddIfNotNullOrEmpty(this IDictionary<string, string> dic, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                dic.Add(key, value);
            }
        }
    }

    public class AuthParameterFactory
    {
        public static SortedDictionary<string, string> GetOAuthParams(string consumerKey,
                                                                      string nonce,
                                                                      string signature,
                                                                      string timestamp,
                                                                      string callback = "",
                                                                      string token = "",
                                                                      string tokenSecret = "",
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
            d.AddIfNotNullOrEmpty(Keys.TOKEN_SECRET, tokenSecret);
            d.AddIfNotNullOrEmpty(Keys.VERIFIER, verifier);
            d.AddIfNotNullOrEmpty(Keys.VERSION, oauthVersion);

            return d;
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
    }
}