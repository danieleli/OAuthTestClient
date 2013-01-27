using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Formatting;
using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client.Composers
{
    /// Composer Pattern - Create msg, Insert Header, Send msg, Extract token
    public static class VerifierTokenComposer
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(VerifierTokenComposer));
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
}