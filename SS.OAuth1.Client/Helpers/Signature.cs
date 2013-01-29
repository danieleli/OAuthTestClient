#region

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using log4net;

#endregion

namespace SS.OAuth1.Client.Helpers
{
    public enum SignatureMethod
    {
        Simple,
        OAuthMAC,
        OAuth1A
    }

    public static class Signature
    {
        internal const string DELIMITER = "\n";
        private static readonly ILog LOG = LogManager.GetLogger(typeof (Signature));

        public static string GetSimpleSignature(HttpRequestMessage request, byte[] apiSecret, string token, string nonce,
                                                string timestamp)
        {
            var normalizedRequest = string.Join(DELIMITER, new[] {token, timestamp, nonce});

            var hmac = new HMACSHA256(apiSecret);
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(normalizedRequest)));
        }

        public static string GetMacSignature(HttpRequestMessage request, byte[] apiSecret, string token, string n,
                                             string ts, string bodyHash)
        {
            var queryString = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var requestMethod = request.Method.ToString().ToUpper();
            var resourcePath = request.RequestUri.AbsolutePath;
            var normalizedParameters = CreateNormalizedParameters(queryString);

            var normalizedRequest = string.Join(DELIMITER,
                                                new[]
                                                    {
                                                        token, ts, n, bodyHash, requestMethod, resourcePath,
                                                        normalizedParameters
                                                    });

            LogMacSig(apiSecret, normalizedRequest);

            var hmac = new HMACSHA256(apiSecret);
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(normalizedRequest)));
        }

        private static void LogMacSig(byte[] apiSecret, string normalizedRequest)
        {
            LOG.Debug("MAC Protected string begin");
            LOG.Debug(normalizedRequest.Replace("\n", Environment.NewLine));
            LOG.Debug("API Secret - Base64 " + Convert.ToBase64String(apiSecret));
            LOG.Debug("API Secret - Hex " + BitConverter.ToString(apiSecret).Replace("-", string.Empty));
            LOG.Debug("MAC Protected string end");
        }

        public static string CreateBodyHash(HttpContent httpContent)
        {
            if (httpContent == null) return null;

            var bodyContent = Encoding.UTF8.GetString(httpContent.ReadAsByteArrayAsync().Result);

            return CreateBodyHash(bodyContent);
        }

        public static string CreateBodyHash(string bodyContent)
        {
            // calculate body hash
            var sha = SHA256.Create();
            var bodyHash = (bodyContent == null)
                               ? null
                               : Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(bodyContent)));

            return bodyHash;
        }

        public static string CreateNormalizedParameters(NameValueCollection queryString)
        {
            // normalize the query string
            var values = new List<string>();
            foreach (string key in queryString.Keys)
            {
                values.Add(Uri.EscapeDataString(string.Concat(key, "=", queryString.Get(key))));
            }

            values.ToList().Sort();
            return string.Join(DELIMITER, values);
        }

        public static string GetOAuth1ASignature(HttpRequestMessage request, string consumerKey, string consumerSecret,
                                                 string accessToken, string accessSecret, string timestamp, string nonce,
                                                 string callback, string verifier)
        {
            return GetOAuth1ASignature(request.RequestUri, request.Method, consumerKey, consumerSecret, accessToken,
                                       accessSecret, timestamp, nonce, callback, verifier);
        }

        public static string GetOAuth1ASignature(Uri requestUri, HttpMethod httpMethod, string consumerKey,
                                                 string consumerSecret,
                                                 string accessToken, string accessSecret, string timestamp, string nonce,
                                                 string callback, string verifier)
        {
            var oauth = new OAuth1ASignature();
            string normalizedurl;
            string normalizedqueryparameters;
            var sig = oauth.GenerateSignature(requestUri, consumerKey, consumerSecret, accessToken, accessSecret,
                                              httpMethod.ToString().ToUpper(), timestamp, nonce,
                                              OAuth1ASignature.SignatureTypes.HMACSHA1, callback, verifier,
                                              out normalizedurl, out normalizedqueryparameters);

            //string sig = oauth.GenerateSignature(request.RequestUri.ChangeToExternalIfRerouted(), consumerKey, consumerSecret, accessToken, accessSecret,
            //    request.Method.ToString().ToUpper(), timestamp, nonce, OAuth1ASignature.SignatureTypes.HMACSHA1, callback, verifier, out normalizedurl, out normalizedqueryparameters);

            return sig;
        }
    }

    public class OAuth1ASignature
    {
        /// <summary>
        ///     Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            HMACSHA1,
            PLAINTEXT,
            RSASHA1
        }

        protected const string OAuthVersion = "1.0";
        protected const string OAuthParameterPrefix = "oauth_";

        //
        // List of know and used oauth parameters' names
        //        
        protected const string OAuthConsumerKeyKey = "oauth_consumer_key";
        protected const string OAuthCallbackKey = "oauth_callback";
        protected const string OAuthVersionKey = "oauth_version";
        protected const string OAuthSignatureMethodKey = "oauth_signature_method";
        protected const string OAuthSignatureKey = "oauth_signature";
        protected const string OAuthTimestampKey = "oauth_timestamp";
        protected const string OAuthNonceKey = "oauth_nonce";
        protected const string OAuthTokenKey = "oauth_token";
        protected const string OAuthTokenSecretKey = "oauth_token_secret";
        protected const string OAuthVerifierKey = "oauth_verifier";

        protected const string HMACSHA1SignatureType = "HMAC-SHA1";
        protected const string PlainTextSignatureType = "PLAINTEXT";
        protected const string RSASHA1SignatureType = "RSA-SHA1";
        private static readonly ILog LOG = LogManager.GetLogger(typeof (OAuth1ASignature));


        protected Random random = new Random();

        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        ///     Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {
            if (hashAlgorithm == null)
            {
                throw new ArgumentNullException("hashAlgorithm");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw new ArgumentNullException("data");
            }

            var dataBuffer = Encoding.ASCII.GetBytes(data);
            var hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        /// <summary>
        ///     Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            var result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                var p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            var temp = s.Split('=');
                            result.Add(new QueryParameter(temp[0], UrlDecode(temp[1])));
                        }
                        else
                        {
                            result.Add(new QueryParameter(s, string.Empty));
                        }
                    }
                }
            }

            return result;
        }

        public string UrlDecode(string value)
        {
            return Uri.UnescapeDataString(value);
        }

        /// <summary>
        ///     This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        ///     While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public string UrlEncode(string value)
        {
            var result = new StringBuilder();

            foreach (char symbol in value)
            {
                if (unreservedChars.IndexOf(symbol) != -1)
                {
                    result.Append(symbol);
                }
                else
                {
                    result.Append('%' + String.Format("{0:X2}", (int) symbol));
                }
            }

            return result.ToString();
        }

        /// <summary>
        ///     Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            var sb = new StringBuilder();
            QueryParameter p = null;
            for (var i = 0; i < parameters.Count; i++)
            {
                p = parameters[i];
                sb.AppendFormat("{0}={1}", p.Name, p.Value);

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        /// <summary>
        ///     Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="token">The requestToken, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The requestToken secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">
        ///     The signature type. To use the default values use <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.
        /// </param>
        /// <returns>The signature base</returns>
        public string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret,
                                            string httpMethod, string timeStamp, string nonce, string signatureType,
                                            string callback, string verifier, out string normalizedUrl,
                                            out string normalizedRequestParameters)
        {
            if (token == null)
            {
                token = string.Empty;
            }

            if (tokenSecret == null)
            {
                tokenSecret = string.Empty;
            }

            if (string.IsNullOrEmpty(consumerKey))
            {
                throw new ArgumentNullException("consumerKey");
            }

            if (string.IsNullOrEmpty(httpMethod))
            {
                throw new ArgumentNullException("httpMethod");
            }

            if (string.IsNullOrEmpty(signatureType))
            {
                throw new ArgumentNullException("signatureType");
            }

            normalizedUrl = null;
            normalizedRequestParameters = null;

            var parameters = GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(OAuthVersionKey, OAuthVersion));
            parameters.Add(new QueryParameter(OAuthNonceKey, nonce));
            parameters.Add(new QueryParameter(OAuthTimestampKey, timeStamp));
            parameters.Add(new QueryParameter(OAuthSignatureMethodKey, signatureType));
            parameters.Add(new QueryParameter(OAuthConsumerKeyKey, consumerKey));
            if (callback != null)
                parameters.Add(new QueryParameter(OAuthCallbackKey, callback)); //UrlDecode(callback)));
            if (verifier != null)
                parameters.Add(new QueryParameter(OAuthVerifierKey, verifier));

            if (!string.IsNullOrEmpty(token))
            {
                parameters.Add(new QueryParameter(OAuthTokenKey, token));
            }

            parameters.Sort(new QueryParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        ///     Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

        /// <summary>
        ///     Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The requestToken, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The requestToken secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token,
                                        string tokenSecret, string httpMethod, string timeStamp, string nonce,
                                        string callback, string verifier, out string normalizedUrl,
                                        out string normalizedRequestParameters)
        {
            return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce,
                                     SignatureTypes.HMACSHA1, callback, verifier, out normalizedUrl,
                                     out normalizedRequestParameters);
        }

        /// <summary>
        ///     Generates a signature using the specified signatureType
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The requestToken, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The requestToken secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token,
                                        string tokenSecret, string httpMethod, string timeStamp, string nonce,
                                        SignatureTypes signatureType, string callback, string verifier,
                                        out string normalizedUrl, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode(string.Format("{0}&{1}", consumerSecret, tokenSecret));
                case SignatureTypes.HMACSHA1:
                    var signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, httpMethod,
                                                              timeStamp, nonce, HMACSHA1SignatureType, callback,
                                                              verifier, out normalizedUrl,
                                                              out normalizedRequestParameters);
                    LOG.Debug("OAUTH.V1A Signature value - " + signatureBase);

                    var hmacsha1 = new HMACSHA1();
                    hmacsha1.Key =
                        Encoding.UTF8.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret),
                                                             string.IsNullOrEmpty(tokenSecret)
                                                                 ? ""
                                                                 : UrlEncode(tokenSecret)));
                    LOG.Debug("OAUTH.V1A Signature key - " +
                              string.Format("{0}&{1}", UrlEncode(consumerSecret),
                                            string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));

                    var signature = GenerateSignatureUsingHash(signatureBase, hmacsha1);
                    LOG.Debug("OAUTH.V1A Signature - " + signature);

                    return signature;
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }

        /// <summary>
        ///     Generate the timestamp for the signature
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        ///     Generate a nonce
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return random.Next(123400, 9999999).ToString();
        }

        /// <summary>
        ///     Provides an internal structure to sort the query parameter
        /// </summary>
        protected class QueryParameter
        {
            private readonly string name;
            private readonly string value;

            public QueryParameter(string name, string value)
            {
                this.name = name;
                this.value = value;
            }

            public string Name
            {
                get { return name; }
            }

            public string Value
            {
                get { return value; }
            }
        }

        /// <summary>
        ///     Comparer class used to perform the sorting of the query parameters
        /// </summary>
        protected class QueryParameterComparer : IComparer<QueryParameter>
        {
            #region IComparer<QueryParameter> Members

            public int Compare(QueryParameter x, QueryParameter y)
            {
                if (x.Name == y.Name)
                {
                    return string.Compare(x.Value, y.Value);
                }
                else
                {
                    return string.Compare(x.Name, y.Name);
                }
            }

            #endregion
        }
    }
}