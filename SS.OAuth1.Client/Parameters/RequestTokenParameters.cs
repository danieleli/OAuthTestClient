using System.Collections.Generic;
using System.Net.Http;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Models;

namespace SS.OAuth1.Client.Parameters
{
    public class RequestTokenParameters : OAuthParametersBase
    {
        public string Callback { get; private set; }
        
        #region -- Constructor --

        public RequestTokenParameters(Creds consumer, string callback = "oob")
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.REQUEST_TOKEN)
        {
            Callback = callback;
        }

        #endregion -- Constructor --

        protected override void AddAuthHeader(HttpRequestMessage msg)
        {
            var authHeader = this.GetAuthHeader();
            msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);
        }

        public string GetAuthHeader()
        {

            var signature = Signature.GetOAuth1ASignature(this.RequestUri,
                                                          this.HttpMethod,
                                                          this.Consumer.Key,
                                                          this.Consumer.Secret,
                                                          null,
                                                          null,
                                                          this.Timestamp,
                                                          this.Nonce,
                                                          this.Callback,
                                                          null);

            var oauthParams = base.GetOAuthParams(this.Consumer.Key,
                                                                  this.Nonce,
                                                                  signature,
                                                                  this.Timestamp,
                                                                  this.Callback);

            var header = "OAuth " + oauthParams.Stringify();

            return header;
        }


    }
}