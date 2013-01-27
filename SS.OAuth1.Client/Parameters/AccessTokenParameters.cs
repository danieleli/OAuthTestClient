#region

using System.Net.Http;

#endregion

namespace SS.OAuth1.Client.Parameters
{
    public class AccessTokenParameters : OAuthParametersBase
    {
        public AccessTokenParameters(Creds consumer, string verifier, string sessionHandle)
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.ACCESS_TOKEN)
        {
            NullCheck(verifier, "oauth_verifier");

            Verifier = verifier;
            SessionHandle = sessionHandle;
        }

        public string Verifier { get; set; }
        public string SessionHandle { get; set; }
    }
}