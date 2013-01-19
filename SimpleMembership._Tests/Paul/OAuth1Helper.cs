using System;
using System.Net.Http;
using log4net;

namespace MXM.API.Test.Controllers
{
    public static class OAuth1Helper
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(OAuth1Helper));


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

    }

    public static class Crypto
    {
        public static HttpRequestMessage SignRequestTokenMsg(HttpRequestMessage msg, Creds consumer, string returnUrl)
        {
            //msg.Sign(SignatureMethod.OAuth1A, consumer.Key, consumer.Secret, otherCreds.Key,
            //         otherCreds.Secret, null, null, returnUrl, null);
            return msg;
        }


        public static HttpRequestMessage SignVerifierMsg(HttpRequestMessage msg, Creds consumer, string token, string verifier)
        {
            //msg.Sign(SignatureMethod.OAuth1A, consumer.Key, consumer.Secret, otherCreds.Key,
            //         otherCreds.Secret, null, null, returnUrl, null);
            return msg;
        }

        public static HttpRequestMessage SignMsg(HttpRequestMessage msg, Creds consumer, Creds user, string state)
        {
            //msg.Sign(SignatureMethod.OAuth1A, consumer.Key, consumer.Secret, otherCreds.Key,
            //         otherCreds.Secret, null, null, returnUrl, null);
            return msg;
        }
    }
}