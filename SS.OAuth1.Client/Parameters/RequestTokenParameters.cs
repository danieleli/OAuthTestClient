using System.Collections.Generic;
using System.Net.Http;
using SS.OAuth1.Client.Helpers;

namespace SS.OAuth1.Client.Parameters
{
    public class RequestTokenParameters : OAuthParametersBase
    {
        public string Callback { get; private set; }

        public RequestTokenParameters(Creds consumer, string callback = "oob")
            : base(consumer, HttpMethod.Get, OAuth.V1.Routes.REQUEST_TOKEN)
        {
            Callback = callback;
        }

        public override string GetOAuthHeader()
        {
            var oauthParamsDictionary = base.OAuthParser.GetOAuthParamsNoSignature(this, callback: this.Callback);
            var signature = base.GetOAuth1ASignature(null, this.Callback);
            oauthParamsDictionary.AddIfNotNullOrEmpty(Keys.SIGNATURE, signature);

            var header = "OAuth " + oauthParamsDictionary.Stringify();

            return header;
        }

    }
}