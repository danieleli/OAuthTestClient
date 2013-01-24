#region

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using PPS.API.Common.Helpers;
using PPS.API.Constants;
using SimpleMembership._Tests.Paul;
using log4net;

#endregion

namespace MXM.API.Test.Controllers
{
    public static class Keys
    {
        public const string OAUTH_VERIFIER = "oauth_verifier";
        public const string TOKEN = "token";
        public const string TOKEN_SECRET = "token_secret";
    }

    public static partial class OAuthRoutes
    {
        public static class V1A
        {
            public const string ROUTE = G.BASE_URL + "/OAuth/1a/";

            public const string REQUEST_TOKEN = ROUTE + "RequestToken";
            public const string TOKEN_VERIFIER = ROUTE + "AuthorizeToken?token={0}&isAuthorized=true";
            public const string ACCESS_TOKEN = ROUTE + "AccessToken";
        }
    }

    public static class OAuth1Helper
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (OAuth1Helper));

        public static Creds GetRequstToken(Creds consumer, string returnUrl)
        {
            LOG.Debug("-----------GetRequestToken-----------");
            Util.LogCreds("Consumer", consumer);
            Util.LogPair("ReturnUrl", returnUrl);

            var msg = MsgHelper.CreateRequestMessage(OAuthRoutes.V1A.REQUEST_TOKEN, HttpMethod.Post);
            msg = Crypto.SignRequestTokenMsg(msg, consumer, returnUrl);
            var response = MsgHelper.Send(msg);

            var requestToken = ExtractToken(response);

            Util.LogCreds("RequestToken", requestToken);
            return requestToken;
        }

        private static Creds ExtractToken(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsFormDataAsync().Result;

            if (result == null) throw new Exception("No Token Returned.");

            var token = new Creds
                {
                    Key = result[Keys.TOKEN],
                    Secret = result[Keys.TOKEN_SECRET]
                };

            return token;
        }

        public static string GetTokenVerifier(string requestToken, Creds consumer)
        {
            LOG.Debug("-----------GetTokenVerifier-----------");
            Util.LogCreds("Consumer", consumer);
            Util.LogPair("RequestToken", requestToken);

            var url = string.Format(OAuthRoutes.V1A.TOKEN_VERIFIER, requestToken);
            var msg = MsgHelper.CreateRequestMessage(url, HttpMethod.Post);
            var response = MsgHelper.Send(msg);

            var verifier = ExtractVerifier(response);
            LOG.Info("Verifier: " + verifier);

            return verifier;
        }

        private static string ExtractVerifier(HttpResponseMessage response)
        {
            if (response.Headers.Location == null)
                return response.Content.ReadAsFormDataAsync().Result[Keys.OAUTH_VERIFIER];

            return response.Headers.Location.ParseQueryString()[Keys.OAUTH_VERIFIER];
        }

        public static Creds GetAccessToken(string token, string verifier, Creds consumer)
        {
            LOG.Debug("-----------GetAccessToken-----------");
            Util.LogCreds("Consumer", consumer);
            Util.LogPair("Verifier", verifier);

            var msg = MsgHelper.CreateRequestMessage(OAuthRoutes.V2.ACCESS_TOKEN, HttpMethod.Post);
            Crypto.SignVerifierMsg(msg, consumer, token, verifier);
            var response = MsgHelper.Send(msg);

            var accessToken = ExtractToken(response);
            Util.LogCreds("AccessToken", accessToken);

            return accessToken;
        }

        private static class Util
        {
            public static void LogCreds(string credType, Creds creds)
            {
                LOG.Info(credType + ": " + creds.Key);
                LOG.Info(credType + "Secret: " + creds.Secret);
            }

            public static void LogPair(string key, string value)
            {
                LOG.Info(key + ": " + value);
            }
        }
    }

    public static class Crypto
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (Crypto));


        // http://tools.ietf.org/html/rfc5849#section-3.4.1.1
        public static HttpRequestMessage SignRequestTokenMsg(HttpRequestMessage msg, Creds consumer, string callback)
        {
            var timestamp = OAuthUtils.GenerateTimeStamp();
            var nonce = OAuthUtils.GenerateNonce();

            var oauthParams = OAuth.V1.GetOAuthParams(callback, consumer.Key, nonce, null,
                                                           OAuth1ASignature.SignatureTypes.HMACSHA1.ToString(),
                                                           timestamp, null, null, null, "1.0");

            var sigBase = GetSignatureBaseString(msg, oauthParams);

            LOG.Debug("Normalized Parameters: " + sigBase);
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

        private static string GetSignatureBaseString(HttpRequestMessage msg, IDictionary<string, string> oauthParams)
        {
            // Spec step 1
            var signatureBaseString = msg.Method.ToString().ToUpper();

            // Spec step 2
            signatureBaseString += "&";

            // Spec step 3
            var baseUri = GetBaseStringUri(msg.RequestUri);
            //var uriBytes = Encoding.UTF8.GetBytes(baseUri);
            var escapedUri = Uri.EscapeDataString(baseUri);
            signatureBaseString += escapedUri;

            // Spec step 4
            signatureBaseString += "&";

            // Spec step 5
            var encodedParams = EncodeParameters(oauthParams);
            var normalizedParams = NormalizeParameters(encodedParams);
            signatureBaseString += normalizedParams;

            return signatureBaseString;
        }

        private static IEnumerable<KeyValuePair<string, string>> EncodeParameters(IEnumerable<KeyValuePair<string, string>>  oauthParams)
        {
            var rtnDic = new Dictionary<string, string>();
            foreach (var p in oauthParams)
            {
                var escapedKey = Uri.EscapeDataString(p.Key);
                var escapedValue = Uri.EscapeDataString(p.Value);
                rtnDic.Add(escapedKey,escapedValue);
            }

            return rtnDic;
        }

        public static string GetBaseStringUri(Uri url)
        {
            
            var scheme = url.Scheme.ToLower();
            var host = url.Host.ToLower();

            var rtnString = string.Format("{0}://{1}", scheme, host);
            var isDefaultPort = (url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443);
            if (!isDefaultPort)
            {
                rtnString += ":" + url.Port;
            }
            rtnString += url.AbsolutePath;

            return rtnString;
        }

        private static string NormalizeParameters(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            var rtnParams = new List<string>();

            foreach (var p in parameters)
            {
                var normalParam = string.Format("{0}=\"{1}\"", p.Key, p.Value);
                var escapedParam = Uri.EscapeDataString(normalParam);
                rtnParams.Add(escapedParam);
            }

            rtnParams.Sort();
            var rtnString = string.Join("&", rtnParams);

            return rtnString;
        }

        public static string GenerateSignatureBase(Uri url, IEnumerable<KeyValuePair<string, string>> oAuthParams)
        {
            var queryStringParams = url.ParseQueryString();

            LOG.Debug("QueryStringParams: " + queryStringParams);
            //queryStringParams.Add(oAuthParams);
            //NormalizeParameters(queryStringParams)
            var normalizedUrl = string.Format("{0}://{1}", url.Scheme, url.Host);
            if (!((url.Scheme == "http" && url.Port == 80) || (url.Scheme == "https" && url.Port == 443)))
            {
                normalizedUrl += ":" + url.Port;
            }
            normalizedUrl += url.AbsolutePath;

            var signatureBase = new StringBuilder();
            //signatureBase.AppendFormat("{0}&", httpMethod.ToUpper());
            //signatureBase.AppendFormat("{0}&", UrlEncode(normalizedUrl));
            //signatureBase.AppendFormat("{0}", UrlEncode(normalizedRequestParameters));

            return signatureBase.ToString();
        }





        public static HttpRequestMessage SignVerifierMsg(HttpRequestMessage msg, Creds consumer, string token,
                                                         string verifier)
        {
            //msg.Sign(SignatureMethod.OAuth1A, consumer.Key, consumer.Secret, otherCreds.Key,
            //         otherCreds.Secret, null, null, callback, null);
            return msg;
        }

        public static HttpRequestMessage SignMsg(HttpRequestMessage msg, Creds consumer, Creds user, string state)
        {
            //msg.Sign(SignatureMethod.OAuth1A, consumer.Key, consumer.Secret, otherCreds.Key,
            //         otherCreds.Secret, null, null, callback, null);
            return msg;
        }
    }
}