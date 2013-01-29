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
            var msg = parameters.CreateRequestMessage();

            var authHeader = parameters.GetAuthHeader();
            msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);

            var sender = new MessageSender();
            var response = sender.Send(msg);
            var requestToken = Util.ExtractToken(response);

            return requestToken;
        }

    }
}