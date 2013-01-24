#region

using System;
using System.Collections.Specialized;
using System.Net.Http;
using MXM.API.Test.Controllers;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1
{
    public static class OAuth1Helper
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (OAuth1Helper));

        public static class AccessTokenHelper
        {
            public static Creds GetAccessToken(Creds consumer, Creds verifierToken)
            {
                LOG.Debug("-----------GetAccessToken-----------");
                Util.LogCreds("Consumer", consumer);
                Util.LogCreds("Verifier", verifierToken);

                var msg = MsgHelper.CreateRequestMessage(OAuthRoutes.V2.ACCESS_TOKEN, HttpMethod.Post);
                Crypto.AccessTokenMessage.Sign(msg, consumer, verifierToken);
                var response = MsgHelper.Send(msg);

                var accessToken = Util.ExtractToken(response);
                Util.LogCreds("AccessToken", accessToken);

                return accessToken;
            }
        }

        public static class RequestTokenHelper
        {
            public static Creds GetRequstToken(Creds consumer, string returnUrl)
            {
                LOG.Debug("-----------GetRequestToken-----------");
                Util.LogCreds("Consumer", consumer);
                Util.LogPair("ReturnUrl", returnUrl);

                var msg = MsgHelper.CreateRequestMessage(OAuth.V1.Routes.REQUEST_TOKEN, HttpMethod.Post);
                msg = Crypto.RequestTokenMessage.Sign(msg, consumer, returnUrl);
                var response = MsgHelper.Send(msg);

                var requestToken = Util.ExtractToken(response);

                Util.LogCreds("RequestToken", requestToken);
                return requestToken;
            }
        }

        private static class Util
        {
            public static Creds ExtractToken(HttpResponseMessage response)
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

        public static class VerifierTokenHelper
        {
            public static Creds GetVerifierToken(Creds requestToken, Creds consumer, Creds user)
            {
                LOG.Debug("-----------GetTokenVerifier-----------");
                Util.LogCreds("Consumer", consumer);
                Util.LogCreds("RequestToken", requestToken);

                var url = OAuth.V1.Routes.GetTokenVerifierRoute(requestToken.Key);
                var msg = MsgHelper.CreateRequestMessage(url, HttpMethod.Post);
                msg = Crypto.VerifierMessage.Sign(msg, consumer, requestToken);
                var response = MsgHelper.Send(msg);

                var verifier = ExtractVerifier(response);
                LOG.Info("Verifier: " + verifier);

                return verifier;
            }


            private static Creds ExtractVerifier(HttpResponseMessage response)
            {
                NameValueCollection result = null;
                if (response.Headers.Location == null)
                {
                    result = response.Content.ReadAsFormDataAsync().Result;
                }
                else
                {
                    result = response.Headers.Location.ParseQueryString();
                }

                var token = new Creds
                    {
                        Key = result[OAuth.V1.Keys.TOKEN],
                        Secret = result[OAuth.V1.Keys.VERIFIER]
                    };

                return token;
            }
        }
    }
}