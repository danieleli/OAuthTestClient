using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client.Composers
{
    // Composer Pattern - Create msg, Insert Header, Send msg, Extract token
    public static class RequestTokenComposer
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(RequestTokenComposer));

        public static Creds GetRequstToken(RequestTokenParameters parameters)
        {
            BeginLog(parameters);

            var msg = parameters.CreateRequestMessage();

            var authHeader = parameters.GetAuthHeader();
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
            LOG.LogCreds("Consumer", parameters.Consumer);
            LOG.LogPair("ReturnUrl", parameters.Callback);
        }

        private static void EndLog(Creds requestToken)
        {
            LOG.LogCreds("RequestToken", requestToken);
            LOG.Debug("-----------End: GetRequestToken-----------");
        }

        #endregion -- logging --
    }
}