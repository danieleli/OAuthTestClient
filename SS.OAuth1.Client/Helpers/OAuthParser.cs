using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using SS.OAuth1.Client.Extensions;
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

        protected string Encode(string s)
        {
            return StringEx.UrlEncodeForOAuth(s);
        }

        protected string Stringify(NameValueCollection pairs)
        {
            return NameValueCollectionEx.Stringify(pairs);
        }


        public string GetSignatureBase(OAuthParametersBase paramz, string callback = null, string token = null, string verifier = null)
        {
            /*
               http://tools.ietf.org/html/rfc5849#section-3.4.1.1
              
               1.  The HTTP request method in uppercase.  For example: "HEAD",
                   "GET", "POST", etc.  If the request uses a custom HTTP method, it
                   MUST be encoded (Section 3.6).

               2.  An "&" character (ASCII code 38).

               3.  The base string URI from Section 3.4.1.2, after being encoded
                   (Section 3.6).

               4.  An "&" character (ASCII code 38).

               5.  The request parameters as normalized in Section 3.4.1.3.2, after
                   being encoded (Section 3.6).              
            */

            var method = paramz.HttpMethod.ToString().ToUpper();
            var baseStringUri = paramz.RequestUri.GetBaseStringUri().UrlEncodeForOAuth();


            var normalizedRequestParams = ""; 

            var rtn = string.Format("{0}&{1}&{2}", method, baseStringUri, normalizedRequestParams);

            return rtn;
        }


        public static string CreateHeader(OAuthParametersBase paramz, Creds requestToken, string callback = null, string verifier = null)
        {
            requestToken = requestToken ?? new Creds(null, null);

            var oauthParamDictionary = paramz.GetOAuthParams();

            var sig = CreateSignature(paramz, requestToken, callback, verifier);

            oauthParamDictionary.Add(Keys.SIGNATURE, sig);

            return oauthParamDictionary.Stringify();
        }


        private static string CreateSignature(OAuthParametersBase paramz, Creds requestToken, string callback = null, string verifier = null)
        {
            string requestTokenKey = null;
            string requestTokenSecret = null;

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