#region

using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Formatting;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1
{
    public static class RequestHelper
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (RequestHelper));

        public static class AccessTokenHelper
        {
            public static Creds GetAccessToken(Creds consumer, Creds verifier)
            {

                var input = new AccessTokenInput(consumer, verifier.Secret, "AAA");
                return GetAccessToken(input);
            }

            public static Creds GetAccessToken(AccessTokenInput input)
            {
                LOG.Debug("-----------Begin: GetAccessToken-----------");
                Util.LogCreds("Consumer", input.Consumer);
                Util.LogPair("Verifier", input.Verifier);

                var msg = MsgHelper.CreateRequestMessage(OAuth.V1.Routes.ACCESS_TOKEN, HttpMethod.Get);
                var authHeader = AuthorizationHeaderFactory.CreateAccessTokenHeader(input);
                msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);
                var response = MsgHelper.Send(msg);

                var accessToken = Util.ExtractToken(response);
                Util.LogCreds("AccessToken", accessToken);
                LOG.Debug("-----------End: GetAccessToken-----------");
                return accessToken;
            }
        }

        public static class RequestTokenHelper
        {
            public static Creds GetRequstToken(RequestTokenInput input)
            {
                LOG.Debug("-----------Begin: GetRequestToken-----------");
                Util.LogCreds("Consumer", input.Consumer);
                Util.LogPair("ReturnUrl", input.Callback);

                var msg = MsgHelper.CreateRequestMessage(input);

                var authHeader = AuthorizationHeaderFactory.CreateRequestTokenHeader(input);
                msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);

                var response = MsgHelper.Send(msg);

                var requestToken = Util.ExtractToken(response);

                
                LOG.Debug("-----------End: GetRequestToken-----------");
                return requestToken;
            }

            [Obsolete]
            public static Creds GetRequstToken(Creds consumer, string callback)
            {
                var input = new RequestTokenInput(consumer, callback);
                return GetRequstToken(input);
            }
        }


        public static class Util
        {
            public static Creds ExtractToken(HttpResponseMessage response)
            {
                var result = response.Content.ReadAsFormDataAsync().Result;

                if (result == null) throw new Exception("No Verifier Returned.");

                var key = result[OAuth.V1.Keys.TOKEN];
                var secret = result[OAuth.V1.Keys.TOKEN_SECRET];
                var token = new Creds(key, secret);
                
                Util.LogCreds("Verifier", token);
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
                LOG.Debug("-----------Begin: GetTokenVerifier-----------");
                Util.LogCreds("Consumer", consumer);
                Util.LogCreds("RequestToken", requestToken);

                var response = GetAuthorizeResponse(requestToken);
                LOG.Debug("Response: " + response);
                var content = response.Content.ReadAsStringAsync().Result;
                LOG.Debug("Response Content: " + content);

                var verifier = ExtractVerifier(response);
                LOG.Info("Verifier: " + verifier);
                LOG.Debug("-----------End: GetTokenVerifier-----------");
                return verifier;
            }

            public static HttpResponseMessage GetAuthorizeResponse(Creds requestToken)
            {
                var url = OAuth.V1.Routes.GetAuthorizeTokenRoute(requestToken.Key);
                var msg = MsgHelper.CreateRequestMessage(url, HttpMethod.Get);
                var response = MsgHelper.Send(msg);
                return response;
            }

            private static Creds ExtractVerifier(HttpResponseMessage response)
            {
                response.EnsureSuccessStatusCode();
                NameValueCollection result = null;

                if (response.Headers.Location == null)
                {
                    var formatter = new FormUrlEncodedMediaTypeFormatter();
                    result = response.Content.ReadAsFormDataAsync().Result;
                }
                else
                {
                    result = response.Headers.Location.ParseQueryString();
                }

                var key = result[OAuth.V1.Keys.TOKEN];
                var secret = result[OAuth.V1.Keys.VERIFIER];
                var token = new Creds(key, secret);

                return token;
            }
        }
    }
}