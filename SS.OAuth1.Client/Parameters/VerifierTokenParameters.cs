using System.Net.Http;

namespace SS.OAuth1.Client.Parameters
{
    public class VerifierTokenParameters : OAuthParametersBase
    {
        public VerifierTokenParameters(Creds consumer, string token)
            : base(consumer, HttpMethod.Get, OAuth.V1.Routes.GetAuthorizeTokenRoute(token))
        {
            
        }

        public override string GetAuthHeader()
        {
            throw new System.NotImplementedException();
        }
    }
}