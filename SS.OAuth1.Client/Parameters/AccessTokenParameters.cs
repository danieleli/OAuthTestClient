#region

using System.Collections.Generic;
using System.Net.Http;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Models;

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

        public override string GetOAuthHeader()
        {
            var oauthParamsDictionary = base.GetOAuthParamsNoSignature(this.Consumer.Key, Verifier, token:this.RequestToken.Key);
            var signature = base.GetOAuth1ASignature(this.RequestToken, Verifier);
            oauthParamsDictionary.AddIfNotNullOrEmpty(Keys.SIGNATURE, signature);

            var header = "OAuth " + oauthParamsDictionary.Stringify();

            return header;
        }

    }
}