#region

using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Formatting;
using SS.OAuth1.Client.Parameters;
using log4net;

#endregion

namespace SS.OAuth1.Client
{
    /// Composer Pattern - Create msg, Insert Header, Send msg, Extract token
    public static class RequestComposer
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (RequestComposer));


        public static class RequestTokenHelper
        {
            public static Creds GetRequstToken(RequestTokenParameters parameters)
            {
                BeginLog(parameters);

                var msg = MessageFactory.CreateRequestMessage(parameters);

                var authHeader = AuthorizationHeaderFactory.CreateRequestTokenHeader(parameters);
                msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);

                var response = MessageSender.Send(msg);
                var requestToken = Util.ExtractToken(response);

                EndLog(requestToken);

                return requestToken;
            }

            #region -- logging --

            private static void BeginLog(RequestTokenParameters parameters)
            {
                LOG.Debug("-----------Begin: GetRequestToken-----------");
                Util.LogCreds("Consumer", parameters.Consumer);
                Util.LogPair("ReturnUrl", parameters.Callback);
            }

            private static void EndLog(Creds requestToken)
            {
                Util.LogCreds("RequestToken", requestToken);
                LOG.Debug("-----------End: GetRequestToken-----------");
            }

            #endregion -- logging --
        }

        public static class VerifierTokenHelper
        {
            public static Creds GetVerifierToken(Creds requestToken, Creds consumer, Creds user)
            {
                LOG.Debug("-----------Begin: GetTokenVerifier-----------");
                Util.LogCreds("Consumer", consumer);
                Util.LogCreds("RequestToken", requestToken);

                var response = GetAuthorizeResponse(consumer, requestToken.Key);
                var content = response.Content.ReadAsStringAsync().Result;

                var verifier = ExtractVerifier(response);
                LOG.Info("Verifier: " + verifier);
                LOG.Debug("-----------End: GetTokenVerifier-----------");
                return verifier;
            }

            public static HttpResponseMessage GetAuthorizeResponse(Creds consumer, string requestToken)
            {
                var input = new VerifierTokenParameters(consumer, requestToken);
                var msg = MessageFactory.CreateRequestMessage(input);
                var response = MessageSender.Send(msg);
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

                var key = result[AuthParameterFactory.Keys.TOKEN];
                var secret = result[AuthParameterFactory.Keys.VERIFIER];
                var token = new Creds(key, secret);

                return token;
            }
        }

        public static class AccessTokenHelper
        {
            [Obsolete]
            public static Creds GetAccessToken(Creds consumer, Creds verifier)
            {
                var input = new AccessTokenParameters(consumer, verifier.Secret, "AAA");
                return GetAccessToken(input);
            }

            public static Creds GetAccessToken(AccessTokenParameters parameters)
            {
                BeginLog(parameters);

                var msg = MessageFactory.CreateRequestMessage(parameters);
                var authHeader = AuthorizationHeaderFactory.CreateAccessTokenHeader(parameters);
                msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);
                var response = MessageSender.Send(msg);

                var accessToken = Util.ExtractToken(response);

                EndLog(accessToken);

                return accessToken;
            }

            #region -- logging --

            private static void EndLog(Creds accessToken)
            {
                Util.LogCreds("AccessToken", accessToken);
                LOG.Debug("-----------End: GetAccessToken-----------");
            }

            private static void BeginLog(AccessTokenParameters parameters)
            {
                LOG.Debug("-----------Begin: GetAccessToken-----------");
                Util.LogCreds("Consumer", parameters.Consumer);
                Util.LogPair("Verifier", parameters.Verifier);
            }

            #endregion -- logging --
        }

        public static class Util
        {
            public static Creds ExtractToken(HttpResponseMessage response)
            {
                var result = response.Content.ReadAsFormDataAsync().Result;

                if (result == null) throw new Exception("No Verifier Returned.");

                var key = result[AuthParameterFactory.Keys.TOKEN];
                var secret = result[AuthParameterFactory.Keys.TOKEN_SECRET];
                var token = new Creds(key, secret);

                LogCreds("Verifier", token);
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

    }
}