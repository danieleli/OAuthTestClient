﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using PPS.API.Constants;

namespace PPS.API.Common.Helpers
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

        public static string GetSimpleSignature(HttpRequestMessage request, byte[] apiSecret, string token, string n, string ts)
        {
            string normalizedRequest = string.Join(DELIMITER, new string[] { token, ts, n });

            HMACSHA256 hmac = new HMACSHA256(apiSecret);
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(normalizedRequest)));
        }

        public static string GetMacSignature(HttpRequestMessage request, byte[] apiSecret, string token, string n, string ts, string bodyHash)
        {
            var queryString = HttpUtility.ParseQueryString(request.RequestUri.Query);
            var requestMethod = request.Method.ToString().ToUpper();
            var resourcePath = request.RequestUri.AbsolutePath;
            var normalizedParameters = Signature.CreateNormalizedParameters(queryString);

            string normalizedRequest = string.Join(DELIMITER, new string[] { token, ts, n, bodyHash, requestMethod, resourcePath, normalizedParameters });

            //Logger.Debug(typeof(Signature), "MAC Protected string begin");
            //Logger.Debug(typeof(Signature), normalizedRequest.Replace("\n", System.Environment.NewLine));
            //Logger.Debug(typeof(Signature), "API Secret - Base64 " + Convert.ToBase64String(apiSecret));
            //Logger.Debug(typeof(Signature), "API Secret - Hex " + BitConverter.ToString(apiSecret).Replace("-", string.Empty));
            //Logger.Debug(typeof(Signature), "MAC Protected string end");

            HMACSHA256 hmac = new HMACSHA256(apiSecret);
            return Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(normalizedRequest)));
        }

        public static string CreateBodyHash(HttpContent httpContent)
        {
            if (httpContent == null) return null;

            string bodyContent = System.Text.UTF8Encoding.UTF8.GetString(httpContent.ReadAsByteArrayAsync().Result);

            return CreateBodyHash(bodyContent);
        }

        public static string CreateBodyHash(string bodyContent)
        {
            // calculate body hash
            SHA256 sha = SHA256Managed.Create();
            var bodyHash = (bodyContent == null) ? null : Convert.ToBase64String(sha.ComputeHash(Encoding.UTF8.GetBytes(bodyContent)));

            return bodyHash;
        }

        public static string CreateNormalizedParameters(NameValueCollection queryString)
        {
            // normalize the query string
            List<string> values = new List<string>();
            foreach (string key in queryString.Keys)
            {
                values.Add(System.Uri.EscapeDataString(string.Concat(key, "=", queryString.Get(key))));
            }

            values.ToList<string>().Sort();
            return string.Join(DELIMITER, values);
        }

        public static string GetOAuth1ASignature(HttpRequestMessage request, string consumerKey, string consumerSecret, string accessToken, string accessSecret, string timestamp, string nonce, string callback, string verifier)
        {
            OAuth1ASignature oauth = new OAuth1ASignature();
            string normalizedurl;
            string normalizedqueryparameters;
            string sig = "tbd";//oauth.GenerateSignature(request.RequestUri.ChangeToExternalIfRerouted(), consumerKey, consumerSecret, accessToken, accessSecret,
            // request.Method.ToString().ToUpper(), ts, n, OAuth1ASignature.SignatureTypes.HMACSHA1, callback, verifier, out normalizedurl, out normalizedqueryparameters);

            return sig;
        }
    }

    public class OAuth1ASignature
    {

        /// <summary>
        /// Provides a predefined set of algorithms that are supported officially by the protocol
        /// </summary>
        public enum SignatureTypes
        {
            HMACSHA1,
            PLAINTEXT,
            RSASHA1
        }

        /// <summary>
        /// Provides an internal structure to sort the query parameter
        /// </summary>
        protected class QueryParameter
        {
            private string name = null;
            private string value = null;

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
        /// Comparer class used to perform the sorting of the query parameters
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

        protected const string OAuthVersion = "1.0";
        protected const string OAuthParameterPrefix = "oauth_";
        protected const string HMACSHA1SignatureType = "HMAC-SHA1";
        protected const string PlainTextSignatureType = "PLAINTEXT";
        protected const string RSASHA1SignatureType = "RSA-SHA1";


        protected string unreservedChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        /// <summary>
        /// Helper function to compute a hash value
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

            byte[] dataBuffer = Encoding.ASCII.GetBytes(data);
            byte[] hashBytes = hashAlgorithm.ComputeHash(dataBuffer);

            var rtn = Convert.ToBase64String(hashBytes);

            return rtn;
        }

        /// <summary>
        /// Internal function to cut out all non oauth query string parameters (all parameters not begining with "oauth_")
        /// </summary>
        /// <param name="parameters">The query string part of the Url</param>
        /// <returns>A list of QueryParameter each containing the parameter name and value</returns>
        private List<QueryParameter> GetQueryParameters(string parameters)
        {
            if (parameters.StartsWith("?"))
            {
                parameters = parameters.Remove(0, 1);
            }

            List<QueryParameter> result = new List<QueryParameter>();

            if (!string.IsNullOrEmpty(parameters))
            {
                string[] p = parameters.Split('&');
                foreach (string s in p)
                {
                    if (!string.IsNullOrEmpty(s) && !s.StartsWith(OAuthParameterPrefix))
                    {
                        if (s.IndexOf('=') > -1)
                        {
                            string[] temp = s.Split('=');
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
        /// This is a different Url Encode implementation since the default .NET one outputs the percent encoding in lower case.
        /// While this is not a problem with the percent encoding spec, it is used in upper case throughout OAuth
        /// </summary>
        /// <param name="value">The value to Url encode</param>
        /// <returns>Returns a Url encoded string</returns>
        public string UrlEncode(string value)
        {
            StringBuilder result = new StringBuilder();

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

        /// <summary>
        /// Normalizes the request parameters according to the spec
        /// </summary>
        /// <param name="parameters">The list of parameters already sorted</param>
        /// <returns>a string representing the normalized parameters</returns>
        protected string NormalizeRequestParameters(IList<QueryParameter> parameters)
        {
            StringBuilder sb = new StringBuilder();
            QueryParameter p = null;
            for (int i = 0; i < parameters.Count; i++)
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
        /// Generate the signature base that is used to produce the signature
        /// </summary>
        /// <param name="url">The full url that needs to be signed including its non OAuth url parameters</param>
        /// <param name="consumerKey">The consumer key</param>        
        /// <param name="token">The token, if available. If not available pass null or an empty string</param>
        /// <param name="tokenSecret">The token secret, if available. If not available pass null or an empty string</param>
        /// <param name="httpMethod">The http method used. Must be a valid HTTP method verb (POST,GET,PUT, etc)</param>
        /// <param name="signatureType">The signature type. To use the default values use <see cref="OAuthBase.SignatureTypes">OAuthBase.SignatureTypes</see>.</param>
        /// <returns>The signature base</returns>
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

            List<QueryParameter> parameters = GetQueryParameters(url.Query);
            parameters.Add(new QueryParameter(OAuth.V1.Keys.VERSION, OAuthVersion));
            parameters.Add(new QueryParameter(OAuth.V1.Keys.NONCE, nonce));
            parameters.Add(new QueryParameter(OAuth.V1.Keys.TIMESTAMP, timeStamp));
            parameters.Add(new QueryParameter(OAuth.V1.Keys.SIGNATURE_METHOD, signatureType));
            parameters.Add(new QueryParameter(OAuth.V1.Keys.CONSUMER_KEY, consumerKey));
            if (callback != null)
                parameters.Add(new QueryParameter(OAuth.V1.Keys.CALLBACK, callback));//UrlDecode(callback)));
            if (verifier != null)
                parameters.Add(new QueryParameter(OAuth.V1.Keys.VERIFIER, verifier));

            if (!string.IsNullOrEmpty(token))
            {
                parameters.Add(new QueryParameter(OAuth.V1.Keys.TOKEN, token));
            }

            parameters.Sort(new QueryParameterComparer());

            normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;
            normalizedRequestParameters = NormalizeRequestParameters(parameters);

            StringBuilder signatureBase = new StringBuilder();
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
                    string signatureBase = GenerateSignatureBase(url, consumerKey, token, tokenSecret, httpMethod, timeStamp, nonce, HMACSHA1SignatureType, callback, verifier, out normalizedUrl, out normalizedRequestParameters);
                    //	Logger.Debug(this, "OAUTH.V1A Signature value - " + signatureBase);

                    HMACSHA1 hmacsha1 = new HMACSHA1();
                    hmacsha1.Key = Encoding.UTF8.GetBytes(string.Format("{0}&{1}", UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));
                    //		Logger.Debug(this, "OAUTH.V1A Signature key - " + string.Format("{0}&{1}", UrlEncode(consumerSecret), string.IsNullOrEmpty(tokenSecret) ? "" : UrlEncode(tokenSecret)));

                    var signature = GenerateSignatureUsingHash(signatureBase, hmacsha1);
                    //	Logger.Debug(this, "OAUTH.V1A Signature - " + signature);

                    return signature;
                case SignatureTypes.RSASHA1:
                    throw new NotImplementedException();
                default:
                    throw new ArgumentException("Unknown signature type", "signatureType");
            }
        }



    }

}
