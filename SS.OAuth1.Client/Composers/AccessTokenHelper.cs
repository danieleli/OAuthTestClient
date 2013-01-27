using System;
using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client.Composers
{
    /// Composer Pattern - Create msg, Insert Header, Send msg, Extract token
    public static class AccessTokenHelper
    {

        private static readonly ILog LOG = LogManager.GetLogger(typeof(AccessTokenHelper));

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
}