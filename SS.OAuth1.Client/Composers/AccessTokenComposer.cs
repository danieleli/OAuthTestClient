using System;
using System.Net.Http;
using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client.Composers
{
    /// Composer Pattern - Create msg, Insert Header, Send msg, Extract token
    public static class AccessTokenComposer
    {

        private static readonly ILog LOG = LogManager.GetLogger(typeof(AccessTokenComposer));

        public static Creds GetAccessToken(AccessTokenParameters parameters)
        {
            //BeginLog(parameters);

            var msg = MessageFactory.CreateRequestMessage(parameters);
            AddAuthHeader(parameters, msg);
            var response = MessageSender.Send(msg);

            var accessToken = Util.ExtractToken(response);

            EndLog(accessToken);

            return accessToken;
        }

        private static void AddAuthHeader(AccessTokenParameters parameters, HttpRequestMessage msg)
        {
            var authHeader = AuthorizationHeaderFactory.CreateAccessTokenHeader(parameters);
            msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);
        }

        #region -- logging --

        private static void EndLog(Creds accessToken)
        {
            Util.LogCreds("AccessToken", accessToken);
            LOG.Debug("-----------End: GetAccessToken-----------");
        }

        #endregion -- logging --
    }
}