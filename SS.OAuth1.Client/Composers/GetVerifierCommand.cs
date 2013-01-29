using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Formatting;
using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client.Composers
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

            var key = result[Keys.TOKEN];
            var secret = result[Keys.VERIFIER];
            var token = new Creds(key, secret);

            return token;
        }
    }
}