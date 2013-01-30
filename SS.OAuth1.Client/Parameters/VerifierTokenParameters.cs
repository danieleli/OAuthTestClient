using System.Net.Http;
using SS.OAuth1.Client.Helpers;

namespace SS.OAuth1.Client.Parameters
{
    public class VerifierTokenParameters : OAuthParametersBase
    {
        public Creds RequestToken { get; set; }

        // Constructor
        public VerifierTokenParameters(Creds consumer, Creds requestToken)
            : base(consumer, HttpMethod.Get, OAuth.V1.Routes.GetAuthorizeTokenRoute(requestToken.Key))
        {
            RequestToken = requestToken;
        }

        public override string GetOAuthHeader()
        {
            var header = base.OAuthParser.CreateHeader(this, this.RequestToken);
            return header;
        }

    }
}