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
        public string SessionHandle { get; set; }

        #region -- Constructor --

        public AccessTokenParameters(Creds consumer, Creds requestToken)
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.ACCESS_TOKEN)
        {
            NullCheck(requestToken, "requestToken");
            NullCheck(requestToken.Key, "requestToken.Key");
            NullCheck(requestToken.Secret, "requestToken.Secret");

            RequestToken = requestToken;
        }

        #endregion -- Constructor --

        protected override void AddAuthHeader(HttpRequestMessage msg)
        {
            var authHeader = this.GetAuthHeader();
            msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);
        }

        protected string GetAuthHeader()
        {
            var signature = Signature.GetOAuth1ASignature(this.RequestUri,
                                              this.HttpMethod,
                                              this.Consumer.Key,
                                              this.Consumer.Secret,
                                              this.RequestToken.Key,
                                              this.RequestToken.Secret,
                                              this.Timestamp,
                                              this.Nonce,
                                              null,
                                              null);

            var oauthParams = base.GetOAuthParams(this.Consumer.Key,
                                                                  this.Nonce,
                                                                  signature,
                                                                  this.Timestamp, 
                                                                  null,
                                                                  this.RequestToken.Key);


            var header = "OAuth " + oauthParams.Stringify();

            return header;

        }

    }
}