using System;
using System.Collections.Generic;
using SS.OAuth1.Client.Parameters;

namespace SS.OAuth1.Client.Helpers
{


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

    public class OAuthParser
    {
        private SortedDictionary<string, string> GetOAuthParamsCore(OAuthParametersBase p)
        {
            var d = new SortedDictionary<string, string>();
            d.AddIfNotNullOrEmpty(Keys.NONCE, p.Nonce);
            d.AddIfNotNullOrEmpty(Keys.SIGNATURE_METHOD, Values.SIGNATURE_METHOD);
            d.AddIfNotNullOrEmpty(Keys.TIMESTAMP, p.Timestamp);
            d.AddIfNotNullOrEmpty(Keys.VERSION, Values.VERSION);
            d.AddIfNotNullOrEmpty(Keys.CONSUMER_KEY, p.Consumer.Key);
            return d;
        }


        private SortedDictionary<string, string> GetOAuthParamsNoSignature(OAuthParametersBase paramz, string callback = "", string token = "", string verifier = "")
        {
            var sortedDictionary = this.GetOAuthParamsCore(paramz);

            sortedDictionary.AddIfNotNullOrEmpty(Keys.CALLBACK, callback);
            sortedDictionary.AddIfNotNullOrEmpty(Keys.TOKEN, token);
            sortedDictionary.AddIfNotNullOrEmpty(Keys.VERIFIER, verifier);

            return sortedDictionary;
        }

        public string CreateHeader(OAuthParametersBase paramz, Creds requestToken, string callback = "",
                                      string verifier = null)
        {
            requestToken = requestToken ?? new Creds(null, null);

            var oauthParamDictionary = this.GetOAuthParamsNoSignature(paramz, callback, requestToken.Key, verifier);

            var sig = this.CreateSignature(paramz, requestToken, callback, verifier);
            oauthParamDictionary.Add(Keys.SIGNATURE, sig);

            return oauthParamDictionary.Stringify();
        }

        private string CreateSignature(OAuthParametersBase paramz, Creds requestToken, string callback = "", string verifier = null)
        {
            string requestTokenKey = null;
            string requestTokenSecret  = null;

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

        private string CreateSignature(SortedDictionary<string, string> oauthParamsNoSignature,
                                                        string consumerSecret,
                                                        string requestTokenSecret = null)
        {
            // todo: create clean implementation of signature feature.
            throw new NotImplementedException();
        }
    }
}