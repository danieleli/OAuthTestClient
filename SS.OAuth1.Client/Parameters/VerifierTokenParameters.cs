using System.Net.Http;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Models;

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
            var oauthParamsDictionary = base.GetOAuthParamsNoSignature(this.Consumer.Key, token: this.RequestToken.Key);
            var signature = GetOAuth1ASignature();
            oauthParamsDictionary.AddIfNotNullOrEmpty(Keys.SIGNATURE, signature);

            var header = "OAuth " + oauthParamsDictionary.Stringify();

            return header;
        }

        public string GetOAuth1ASignature()
        {
            var signature = Signature.GetOAuth1ASignature(base.RequestUri,
                                                base.HttpMethod,
                                                base.Consumer.Key,
                                                base.Consumer.Secret,
                                                this.RequestToken.Key,
                                                this.RequestToken.Secret,
                                                base.Timestamp,
                                                base.Nonce,
                                                null,
                                                null);
            return signature;
            
        }
    }
}