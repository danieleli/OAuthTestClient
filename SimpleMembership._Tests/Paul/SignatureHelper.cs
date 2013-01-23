using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using log4net;

namespace Temp
{
    public class G
    {
        public const string DELIMITER = "\n";
    }

    public static class SignatureHelper
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SignatureHelper));

        public static string CreateSignature(byte[] apiSecret, string token, string n, string ts, string delimiter = G.DELIMITER)
        {
            var normalizedRequest = String.Join(delimiter, new[] { token, ts, n });
            var utf8Bytes = Encoding.UTF8.GetBytes(normalizedRequest);
            var hmac = new HMACSHA256(apiSecret);
            var hash = hmac.ComputeHash(utf8Bytes);
            var rtn = Convert.ToBase64String(hash);

            return rtn;
        }

        public static string[] BuildParamArray(HttpRequestMessage request, string token, string n,string ts, string delimiter = G.DELIMITER)
        {
            var queryParams = request.GetQueryNameValuePairs();
            var normalizedParameters = CreateNormalizedParameters(queryParams, delimiter);

            var requestMethod = request.Method.ToString().ToUpper();
            var resourcePath = request.RequestUri.AbsolutePath;

            var bodyContent = GetBodyContent(request.Content);
            var bodyHash = GetBase64Hash(bodyContent);

            var paramz = new[] { token, ts, n, bodyHash, requestMethod, resourcePath, normalizedParameters };

            return paramz;
        }

        public static string GetHmacSignature(string[] paramz, byte[] apiSecret, string delimiter = "\n")
        {
            var normalizedParams = string.Join(delimiter, paramz);
            var bytes = Encoding.UTF8.GetBytes(normalizedParams);
            var hmac = new HMACSHA256(apiSecret);

            var hash = hmac.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }

        private static string CreateNormalizedParameters(IEnumerable<KeyValuePair<string, string>> paramz, string delimiter = G.DELIMITER)
        {
            var rtnValues = new List<string>();
            foreach (var item in paramz)
            {
                var paramAsString = string.Concat(item.Key, "=", item.Value);
                var escapedString = Uri.EscapeDataString(paramAsString);
                rtnValues.Add(escapedString);
            }

            rtnValues.ToList().Sort();
            var rtn = string.Join(delimiter, rtnValues);

            return rtn;
        }

        public static void LogThis(byte[] apiSecret, string normalizedRequest)
        {
            Logger.Debug("MAC Protected string begin");
            Logger.Debug(normalizedRequest.Replace("\n", Environment.NewLine));
            Logger.Debug("API Secret - Base64 " + Convert.ToBase64String(apiSecret));
            Logger.Debug("API Secret - Hex " + BitConverter.ToString(apiSecret).Replace("-", string.Empty));
            Logger.Debug("MAC Protected string end");
        }

        public static string GetBodyContent(HttpContent httpContent)
        {
            if (httpContent == null) return null;

            var bodyContent = Encoding.UTF8.GetString(httpContent.ReadAsByteArrayAsync().Result);
         
            return bodyContent;
        }

        public static string GetBase64Hash(string input)
        {
            if (input == null) return null;

            var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha.ComputeHash(bytes);
            var rtn = Convert.ToBase64String(hash);

            return rtn;
        }

        public static string GetOAuth1ASignature(HttpRequestMessage request, string consumerKey, string consumerSecret, string accessToken, string accessSecret, string ts, string n, string callback, string verifier)
        {
            var oauth = new OAuth1ASignature();
            string normalizedurl;
            string normalizedqueryparameters;
            //string sig = oauth.GenerateSignature(request.RequestUri.ChangeToExternalIfRerouted(), consumerKey, consumerSecret, accessToken, accessSecret,
            //    request.Method.ToString().ToUpper(), ts, n, OAuth1ASignature.SignatureTypes.HMACSHA1, callback, verifier, out normalizedurl, out normalizedqueryparameters);

            return null;
            //return sig;
        }
    }

    public class OAuth1ASignature
    {
        private static readonly ILog Logger = LogManager.GetLogger(typeof(SignatureHelper));

        /// <summary>
        /// Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            HMACSHA1,
            PLAINTEXT,
            RSASHA1
        }

        protected const string O_AUTH_VERSION = "1.0";
        protected const string O_AUTH_PARAMETER_PREFIX = "oauth_";

        //
        // List of know and used oauth parameters' names
        //        
        protected const string O_AUTH_CONSUMER_KEY_KEY = "oauth_consumer_key";
        protected const string O_AUTH_CALLBACK_KEY = "oauth_callback";
        protected const string O_AUTH_VERSION_KEY = "oauth_version";
        protected const string O_AUTH_SIGNATURE_METHOD_KEY = "oauth_signature_method";
        protected const string O_AUTH_SIGNATURE_KEY = "oauth_signature";
        protected const string O_AUTH_TIMESTAMP_KEY = "oauth_timestamp";
        protected const string O_AUTH_NONCE_KEY = "oauth_nonce";
        protected const string O_AUTH_TOKEN_KEY = "oauth_token";
        protected const string O_AUTH_TOKEN_SECRET_KEY = "oauth_token_secret";
        protected const string O_AUTH_VERIFIER_KEY = "oauth_verifier";

        public struct SignatureTypez
        {
            public const string HMACSHA1_SIGNATURE_TYPE = "HMAC-SHA1";
            public const string PLAIN_TEXT_SIGNATURE_TYPE = "PLAINTEXT";
            public const string RSASHA1_SIGNATURE_TYPE = "RSA-SHA1";
        }


        protected Random random = new Random();

        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Helper function to compute a hash value
        /// </summary>
        /// <param name="hashAlgorithm">The hashing algoirhtm used. If that algorithm needs some initialization, like HMAC and its derivatives, they should be initialized prior to passing it to this function</param>
        /// <param name="data">The data to hash</param>
        /// <returns>a Base64 string of the hash value</returns>
        private string ComputeHash(HashAlgorithm hashAlgorithm, string data)
        {

            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            return Convert.ToBase64String(hashBytes);
        }

        public IEnumerable<KeyValuePair<string, string>> GetOAuthQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            var result = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(O_AUTH_PARAMETER_PREFIX))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
                //            result.Add(new QueryParameter(temp[0], UrlDecode(temp[1])));
                        }
                        else
                        {
                  //          result.Add(new QueryParameter(s, string.Empty));
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
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
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
                    result.Append('%' + String.Format("{0:X2}", (int)symbol));
                }
            }

            return result.ToString();
        }


        protected string NormalizeRequestParameters(NameValueCollection parameters)
        {
            var sb = new StringBuilder();
            for (int i = 0; i < parameters.Count; i++)
            {
                sb.AppendFormat("{0}={1}", parameters.Keys[i], parameters.GetValues(i));

                if (i < parameters.Count - 1)
                {
                    sb.Append("&");
                }
            }

            return sb.ToString();
        }

        public string GenerateSignatureBase(Uri url, string consumerKey, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string signatureType, string callback, string verifier, out string normalizedUrl, out string normalizedRequestParameters)
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

            //List<QueryParameter> parameters = GetQueryParameters(url);
            //parameters.Add(new QueryParameter(O_AUTH_VERSION_KEY, O_AUTH_VERSION));
            //parameters.Add(new QueryParameter(O_AUTH_NONCE_KEY, nonce));
            //parameters.Add(new QueryParameter(O_AUTH_TIMESTAMP_KEY, timeStamp));
            //parameters.Add(new QueryParameter(O_AUTH_SIGNATURE_METHOD_KEY, signatureType));
            //parameters.Add(new QueryParameter(O_AUTH_CONSUMER_KEY_KEY, consumerKey));
            //if (callback != null)
            //    parameters.Add(new QueryParameter(O_AUTH_CALLBACK_KEY, callback));//UrlDecode(callback)));
            //if (verifier != null)
            //    parameters.Add(new QueryParameter(O_AUTH_VERIFIER_KEY, verifier));

            //if (!string.IsNullOrEmpty(token))
            //{
            //    parameters.Add(new QueryParameter(O_AUTH_TOKEN_KEY, token));
            //}

            //parameters.Sort(new QueryParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = null;// NormalizeRequestParameters(parameters);

            var signatureBase = new StringBuilder();
            signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }

        /// <summary>
        /// Generate the signature value based on the given signature base and hash algorithm
        /// </summary>
        /// <param name="signatureBase">The signature based as produced by the GenerateSignatureBase method or by any other means</param>
        /// <param name="hash">The hash algorithm used to perform the hashing. If the hashing algorithm requires initialization or a key it should be set prior to calling this method</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignatureUsingHash(string signatureBase, HashAlgorithm hash)
        {
            return ComputeHash(hash, signatureBase);
        }

        /// <summary>
        /// Generates a signature using the HMAC-SHA1 algorithm
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, string callback, string verifier, out string normalizedUrl, out string normalizedRequestParameters)
        {
            return GenerateSignature(url, consumerKey, consumerSecret, token, tokenSecret, httpMethod, timeStamp, nonce, SignatureTypes.HMACSHA1, callback, verifier, out normalizedUrl, out normalizedRequestParameters);
        }

        /// <summary>
        /// Generates a signature using the specified signatureType 
        /// </summary>		
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>
        /// <param name="consumerSecret">The consumer seceret</param>
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The type of signature to use</param>
        /// <returns>A base64 string of the hash value</returns>
        public string GenerateSignature(Uri url, string consumerKey, string consumerSecret, string token, string tokenSecret, string httpMethod, string timeStamp, string nonce, SignatureTypes signatureType, string callback, string verifier, out string normalizedUrl, out string normalizedRequestParameters)
        {
            normalizedUrl = null;
            normalizedRequestParameters = null;

            switch (signatureType)
            {
                case SignatureTypes.PLAINTEXT:
                    return HttpUtility.UrlEncode(string.Format("{0}&{1}", consumerSecret, tokenSecret));

                case SignatureTypes.HMACSHA1:
                    string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, "HMACSHA1_SIGNATURE_TYPE", callback, verifier, out normalizedUrl, out normalizedRequestParameters);
                    Logger.Debug("OAUTH.V1A SignatureHelper value - " + signatureBase);

                    var hmacsha1 = new HMACSHA1();
                    hmacsha1.Key = Encoding.UTF8.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));
                    Logger.Debug("OAUTH.V1A SignatureHelper key - " + string.Format("{0}&{1}", UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));

                    var signature = GenerateSignatureUsingHash(signatureBase, hmacsha1);
                    Logger.Debug("OAUTH.V1A SignatureHelper - " + signature);

                    return signature;

                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();

                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }

        /// <summary>
        /// Generate the timestamp for the signature        
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        /// <summary>
        /// Generate a nonce
        /// </summary>
        /// <returns></returns>
        public virtual string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return random.Next(123400, 9999999).ToString();
        }

    }
}
