using System.Net.Http;

namespace SS.OAuth1.Client.Parameters
{
    public class RequestTokenParameters : OAuthParametersBase
    {
        public RequestTokenParameters(Creds consumer, string callback = "oob")
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.REQUEST_TOKEN)
        {
            Callback = callback;
        }

        public string Callback { get; private set; }
        
        public override string GetAuthHeader()
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

            var oauthParams = AuthParameterFactory.GetOAuthParams(this.Consumer.Key,
                                                                  this.Nonce,
                                                                  signature,
                                                                  this.Timestamp,
                                                                  this.Callback);
            
            var header = "OAuth " + oauthParams.Stringify();

            return header;
        }
    }
}