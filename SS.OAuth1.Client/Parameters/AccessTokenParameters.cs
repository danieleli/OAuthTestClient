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
        private readonly string _verifier;
        public Creds RequestToken { get; set; }
        public string SessionHandle { get; set; }

        #region -- Constructor --

        public AccessTokenParameters(Creds consumer, Creds requestToken, string verifier)
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.ACCESS_TOKEN)
        {
            _verifier = verifier;
            NullCheck(requestToken, "requestToken");
            NullCheck(requestToken.Key, "requestToken.Key");
            NullCheck(requestToken.Secret, "requestToken.Secret");

            RequestToken = requestToken;
        }

        #endregion -- Constructor --

        public override string GetOAuthHeader()
        {
            var oauthParamsDictionary = base.GetOAuthParamsNoSignature(this.Consumer.Key, verifier:this._verifier, token:this.RequestToken.Key);
            var signature = GetOAuth1ASignature();
            oauthParamsDictionary.AddIfNotNullOrEmpty(Keys.SIGNATURE, signature);

            var header = "OAuth " + oauthParamsDictionary.Stringify();

            return header;
        }

        public override string GetOAuth1ASignature()
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
                                           _verifier);
            return signature;
        }
    }
}