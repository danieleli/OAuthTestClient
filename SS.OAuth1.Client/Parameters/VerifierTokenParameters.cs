using System.Collections.Specialized;
using System.Net.Http;
using SS.OAuth1.Client.Extensions;
using SS.OAuth1.Client.Helpers;

namespace SS.OAuth1.Client.Parameters
{
    public class VerifierTokenParameters : OAuthParametersBase
    {
        public Creds RequestToken { get; set; }

        // Constructor
        public VerifierTokenParameters(Creds consumer, Creds requestToken, string userToken)
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.GetAuthorizeTokenRoute(userToken))
        {
            RequestToken = requestToken;
        }

        public override NameValueCollection GetOAuthParams()
        {
            var paramPairs = base.GetOAuthParamsCore();
            paramPairs.AddIfNotNullOrEmpty(OAuth.V1.Keys.TOKEN, this.RequestToken.Key);
            
            return paramPairs;
        }

        public override string GetOAuthHeader()
        {
            return base.GetOAuthHeader(this.RequestToken);
        }
    }
}