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

        public override string GetOAuthHeader()
        {
            var oauthParamsDictionary = base.GetOAuthParamsNoSignature(this.Consumer.Key, this.Callback);
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
                                              null,
                                              null,
                                              base.Timestamp,
                                              base.Nonce,
                                              this.Callback,
                                              null);
            return signature;
        }
    }
}