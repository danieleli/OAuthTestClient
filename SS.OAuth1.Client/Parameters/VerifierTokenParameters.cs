using System.Net.Http;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Models;

namespace SS.OAuth1.Client.Parameters
{
    public class VerifierTokenParameters : OAuthParametersBase
    {
        public VerifierTokenParameters(Creds consumer, string requestToken)
            : base(consumer, HttpMethod.Get, OAuth.V1.Routes.GetAuthorizeTokenRoute(requestToken)) { }

        public override string GetOAuthHeader()
        {
            throw new System.NotImplementedException();
        }

        public override string GetOAuth1ASignature()
        {
            throw new System.NotImplementedException();
        }
    }
}