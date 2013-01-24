using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using MXM.API.Test.Controllers;
using PPS.API.Common.Helpers;
using log4net;

namespace SimpleMembership._Tests.Paul.OAuth1
{
    public static class Crypto
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (Crypto));

        // http://tools.ietf.org/html/rfc5849#section-3.4.1.1
        public static HttpRequestMessage SignRequestTokenMsg(HttpRequestMessage msg, Creds consumer, string callback)
        {
            var timestamp = OAuthUtils.GenerateTimeStamp();
            var nonce = OAuthUtils.GenerateNonce();

            
            
            var oauthParams = OAuth.V1.GetOAuthParams(callback, consumer.Key, nonce, null, timestamp, null, null, null);

            var sig = Signature.GetOAuth1ASignature(msg.RequestUri, msg.Method, consumer.Key, consumer.Secret, null, null, timestamp, nonce,
                                                    callback, null);

            oauthParams.Add(OAuth.V1.Keys.SIGNATURE, sig);


         
            var authHeader = Stringify(oauthParams);
            LOG.Debug("authHeader: " + authHeader);
            msg.Headers.Add("Authorization", "OAuth " + authHeader);
            //msg.Headers.Add("Authorization", "OAuth " + authHeader);
            //var signature = oauth1a.GenerateSignature(msg.RequestUri, consumer.Key, consumer.Secret,null, null, msg.Method.ToString() ,timestamp, nonce, OAuth1ASignature.SignatureTypes.HMACSHA1, callback, null,
            //                          out normalizedUrl, out normalizedRequestParams);
            //LOG.Debug("Signature: " + signature);
            //LOG.Debug("url: " + normalizedUrl);
            //LOG.Debug("params: " + normalizedRequestParams);
            //signature = OAuth.V1.Keys.SIGNATURE + "=" + Uri.EscapeDataString(signature);
            //var header = signature + normalizedRequestParams;
            //msg.Headers.Add("Authorization", header);
            //msg.Sign(SignatureMethod.OAuth1A, consumer.Key, consumer.Secret, otherCreds.Key,
            //         otherCreds.Secret, null, null, callback, null);
            return msg;
        }

        public static string Stringify(SortedDictionary<string, string> paramz)
        {
            
            var sb = new StringBuilder();
            var isFirstItem = true;
            foreach (var p in paramz)
            {
                if (!isFirstItem)
                {
                    sb.Append(",");
                }
                var key = Uri.EscapeDataString(p.Key);
                var value = Uri.EscapeDataString(p.Value); 
                sb.Append(string.Format("{0}=\"{1}\"", key, value));
                isFirstItem = false;
            }

            return sb.ToString();
        }

        //private static string GetSignatureBaseString(HttpRequestMessage msg, IDictionary<string, string> oauthParams)
        //{
        //    // Spec step 1
        //    var signatureBaseString = msg.Method.ToString().ToUpper();

        //    // Spec step 2
        //    signatureBaseString += "&";

        //    // Spec step 3
        //    var baseUri = GetBaseStringUri(msg.RequestUri);
        //    //var uriBytes = Encoding.UTF8.GetBytes(baseUri);
        //    var escapedUri = Uri.EscapeDataString(baseUri);
        //    signatureBaseString += escapedUri;

        //    // Spec step 4
        //    signatureBaseString += "&";

        //    // Spec step 5
        //    var encodedParams = EncodeParameters(oauthParams);
        //    var normalizedParams = NormalizeParameters(encodedParams);
        //    signatureBaseString += normalizedParams;

        //    return signatureBaseString;
        //}

        //private static IEnumerable<KeyValuePair<string, string>> EncodeParameters(IEnumerable<KeyValuePair<string, string>>  oauthParams)
        //{
        //    var rtnDic = new Dictionary<string, string>();
        //    foreach (var p in oauthParams)
        //    {
        //        var escapedKey = Uri.EscapeDataString(p.Key);
        //        var escapedValue = Uri.EscapeDataString(p.Value);
        //        rtnDic.Add(escapedKey,escapedValue);
        //    }

        //    return rtnDic;
        //}

        //public static string GetBaseStringUri(Uri url)
        //{
            
        //    var scheme = url.Scheme.ToLower();
        //    var host = url.Host.ToLower();

        //    var rtnString = string.Format("{0}://{1}", scheme, host);
        //    var isDefaultPort = (url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443);
        //    if (!isDefaultPort)
        //    {
        //        rtnString += ":" + url.Port;
        //    }
        //    rtnString += url.AbsolutePath;

        //    return rtnString;
        //}

        //private static string NormalizeParameters(IEnumerable<KeyValuePair<string, string>> parameters)
        //{
        //    var rtnParams = new List<string>();

        //    foreach (var p in parameters)
        //    {
        //        var normalParam = string.Format("{0}=\"{1}\"", p.Key, p.Value);
        //        var escapedParam = Uri.EscapeDataString(normalParam);
        //        rtnParams.Add(escapedParam);
        //    }

        //    rtnParams.Sort();
        //    var rtnString = string.Join("&", rtnParams);

        //    return rtnString;
        //}

        //public static string GenerateSignatureBase(Uri url, IEnumerable<KeyValuePair<string, string>> oAuthParams)
        //{
        //    var queryStringParams = url.ParseQueryString();

        //    LOG.Debug("QueryStringParams: " + queryStringParams);
        //    //queryStringParams.Add(oAuthParams);
        //    //NormalizeParameters(queryStringParams)
        //    var normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
        //    if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
        //    {
        //        normalizedUrl += ":" + url.Port;
        //    }
        //    normalizedUrl += url.AbsolutePath;

        //    var signatureBase = new StringBuilder();
        //    //signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
        //    //signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
        //    //signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

        //    return signatureBase.ToString();
        //}





        public static HttpRequestMessage SignVerifierMsg(HttpRequestMessage msg, Creds consumer, string token,
                                                         string verifier)
        {
            //msg.Sign(SignatureMethod.OAuth1A, consumer.Key, consumer.Secret, otherCreds.Key,
            //         otherCreds.Secret, null, null, callback, null);
            //return msg;
            throw new NotImplementedException();
            
        }

        public static HttpRequestMessage SignMsg(HttpRequestMessage msg, Creds consumer, Creds user, string state)
        {
            //msg.Sign(SignatureMethod.OAuth1A, consumer.Key, consumer.Secret, otherCreds.Key,
            //         otherCreds.Secret, null, null, callback, null);
            //return msg;
            throw new NotImplementedException();
        }
    }
}