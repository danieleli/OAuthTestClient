using System.Collections.Specialized;
using System.Net.Http;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client.Commands
{
    /// Composer Pattern - Create msg, Insert Header, Send msg, Extract token
    public class GetVerifierCommand : GetTokenCommand
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(GetVerifierCommand));
     
        protected override Creds ExtractToken(HttpResponseMessage response)
        {
            response.EnsureSuccessStatusCode();
            NameValueCollection result = null;

            if (response.Headers.Location == null)
            {
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