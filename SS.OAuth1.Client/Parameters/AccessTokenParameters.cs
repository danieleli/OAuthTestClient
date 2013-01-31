#region

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using SS.OAuth1.Client.Extensions;
using SS.OAuth1.Client.Helpers;

#endregion

namespace SS.OAuth1.Client.Parameters
{
    public class AccessTokenParameters : OAuthParametersBase
    {
        
        public Creds RequestToken { get; set; }
        public string Verifier { get; set; }
        public string SessionHandle { get; set; }

        public AccessTokenParameters(Creds consumer, Creds requestToken, string verifier)
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.ACCESS_TOKEN)
        {
        
            NullCheck(requestToken, "requestToken");
            NullCheck(requestToken.Key, "requestToken.Key");
            NullCheck(requestToken.Secret, "requestToken.Secret");

            RequestToken = requestToken;
            Verifier = verifier;
        }

        public override NameValueCollection GetOAuthParams()
        {
            var paramPairs = base.GetOAuthParamsCore();
            paramPairs.AddIfNotNullOrEmpty(Keys.TOKEN, this.RequestToken.Key);
            paramPairs.AddIfNotNullOrEmpty(Keys.VERIFIER, this.Verifier);

            return paramPairs;
        }

        public override string GetOAuthHeader()
        {
            return OAuthParser.CreateHeader(this, this.RequestToken, this.Verifier);
        }
    }
}