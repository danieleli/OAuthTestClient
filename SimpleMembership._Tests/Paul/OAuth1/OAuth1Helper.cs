#region

using System;
using System.Net.Http;
using MXM.API.Test.Controllers;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1
{


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
                    Key = result[OAuth.V1.Keys.TOKEN],
                    Secret = result[OAuth.V1.Keys.TOKEN_SECRET]
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
                return response.Content.ReadAsFormDataAsync().Result[OAuth.V1.Keys.VERIFIER];

            return response.Headers.Location.ParseQueryString()[OAuth.V1.Keys.VERIFIER];
        }

        public static Creds GetAccessToken(string token, string verifier, Creds consumer)
        {
            LOG.Debug("-----------GetAccessToken-----------");
            Util.LogCreds("Consumer", consumer);
            Util.LogPair("Verifier", verifier);

            var msg = MsgHelper.CreateRequestMessage(Paul.OAuthRoutes.V2.ACCESS_TOKEN, HttpMethod.Post);
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
}