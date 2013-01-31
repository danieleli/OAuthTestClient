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
        private NameValueCollection GetOAuthParamsCore(OAuthParametersBase p)
        {
            var d = new NameValueCollection();
            d.AddIfNotNullOrEmpty(Keys.NONCE, p.Nonce);
            d.AddIfNotNullOrEmpty(Keys.SIGNATURE_METHOD, Values.SIGNATURE_METHOD);
            d.AddIfNotNullOrEmpty(Keys.TIMESTAMP, p.Timestamp);
            d.AddIfNotNullOrEmpty(Keys.VERSION, Values.VERSION);
            d.AddIfNotNullOrEmpty(Keys.CONSUMER_KEY, p.Consumer.Key);
            return d;
        }


        private NameValueCollection GetOAuthParamsNoSignature(OAuthParametersBase paramz, string callback = null, string token = null, string verifier = null)
        {
            var paramPairs = this.GetOAuthParamsCore(paramz);

            paramPairs.AddIfNotNullOrEmpty(Keys.CALLBACK, callback);
            paramPairs.AddIfNotNullOrEmpty(Keys.TOKEN, token);
            paramPairs.AddIfNotNullOrEmpty(Keys.VERIFIER, verifier);

            return paramPairs;
        }

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
               http://tools.ietf.org/html/rfc5849#section-3.4.1
            
               3.4.1.  Signature Base String - 

               The signature base string is a consistent, reproducible concatenation
               of several of the HTTP request elements into a single string.  The
               string is used as an input to the "HMAC-SHA1" and "RSA-SHA1"
               signature methods.

               The signature base string includes the following components of the
               HTTP request:

               o  The HTTP request method (e.g., "GET", "POST", etc.).

               o  The authority as declared by the HTTP "Host" request header field.

               o  The path and query components of the request resource URI.

               o  The protocol parameters excluding the "oauth_signature".

               o  Parameters included in the request entity-body if they comply with
                  the strict restrictions defined in Section 3.4.1.3.
                  http://tools.ietf.org/html/rfc5849#section-3.4.1.3

               The signature base string does not cover the entire HTTP request.
               Most notably, it does not include the entity-body in most requests,
               nor does it include most HTTP entity-headers.  It is important to
               note that the server cannot verify the authenticity of the excluded
               request components without using additional protections such as SSL/
               TLS or other methods.
            */

            var method = paramz.HttpMethod.ToString().ToUpper();
            var authority = paramz.RequestUri.Authority;
            var baseStringUri = GetBaseStringUri(paramz.RequestUri);
            var oauthPairs = this.GetOAuthParamsNoSignature(paramz, callback, token, verifier);
            var oauthParams = Stringify(oauthPairs);
            var bodyParams = "";

            return method + "&" + baseStringUri + "&" + oauthParams + "&" + bodyParams;
        }

        private object GetBaseStringUri(Uri requestUri)
        {
            throw new NotImplementedException();
        }


        public string CreateHeader(OAuthParametersBase paramz, Creds requestToken, string callback = null, string verifier = null)
        {
            requestToken = requestToken ?? new Creds(null, null);

            var oauthParamDictionary = this.GetOAuthParamsNoSignature(paramz, callback, requestToken.Key, verifier);

            var sig = this.CreateSignature(paramz, requestToken, callback, verifier);
            oauthParamDictionary.Add(Keys.SIGNATURE, sig);

            return oauthParamDictionary.Stringify();
        }


        private string CreateSignature(OAuthParametersBase paramz, Creds requestToken, string callback = null, string verifier = null)
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