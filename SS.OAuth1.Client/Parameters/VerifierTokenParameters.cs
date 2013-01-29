using System.Net.Http;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Models;

namespace SS.OAuth1.Client.Parameters
{
    public class VerifierTokenParameters : OAuthParametersBase
    {
        public VerifierTokenParameters(Creds consumer, string token)
            : base(consumer, HttpMethod.Get, OAuth.V1.Routes.GetAuthorizeTokenRoute(token)) { }

        public string GetAuthHeader()
        {
            throw new System.NotImplementedException();
        }

        protected override void AddAuthHeader(HttpRequestMessage msg)
        {
            var authHeader = this.GetAuthHeader();
            msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);
        }
    }
}