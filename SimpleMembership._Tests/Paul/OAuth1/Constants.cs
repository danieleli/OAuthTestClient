#region

using System.Collections.Generic;
using MXM.API.Test.Controllers;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1
{
    public static class Extensions
    {
        public static void AddIfNotNull(this IDictionary<string, string> dic, string key, string value)
        {
            if (value == null) return;
            dic.Add(key, value);
        }
    }

    public static class OAuth
    {
        public static class V1
        {
            // List of know and used oauth parameters' names
            public static SortedDictionary<string, string> GetOAuthParams(string callback, 
                                                                            string consumerKey,
                                                                            string nonce, 
                                                                            string signature,
                                                                            string timestamp, 
                                                                            string token,
                                                                            string tokenSecret, 
                                                                            string verifier,            
                                                                            string oauthVersion = Values.VERSION,
                                                                            string signatureMethod = Values.SIGNATURE_METHOD)
            {
                var d = new SortedDictionary<string, string>();

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

            public static class Routes
            {
                public const string ROUTE = G.BASE_URL + "/OAuth/1a/";

                public const string REQUEST_TOKEN = ROUTE + "RequestToken";
                public const string TOKEN_VERIFIER = ROUTE + "AuthorizeToken?token={0}&isAuthorized=true";
                public const string ACCESS_TOKEN = ROUTE + "AccessToken";

                public static string GetTokenVerifierRoute(string token)
                {
                    return string.Format(TOKEN_VERIFIER, token);
                }
            }

            public class Values
            {
                public const string VERSION = "1.0";
                public const string SIGNATURE_METHOD = "HMAC-SHA1";
            }
        }
    }
}