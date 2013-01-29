using System.Collections.Generic;
using System.Net.Http;

namespace SS.OAuth1.Client.Parameters
{
    public class RequestTokenParameters : OAuthParametersBase
    {
        #region -- Constructor --

        public RequestTokenParameters(Creds consumer, string callback = "oob")
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.REQUEST_TOKEN)
        {
            Callback = callback;
        }

        #endregion -- Constructor --

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

        private SortedDictionary<string, string> ToSortedDictionary()
        {
            var d =  new SortedDictionary<string, string>();
             
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.CONSUMER_KEY,this.Consumer.Key);
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.NONCE, this.Nonce);
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.SIGNATURE_METHOD, SIGNATURE_METHOD);
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.TIMESTAMP, this.Timestamp);
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.VERSION, VERSION);
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.CALLBACK, this.Callback);
            return d;
       
        
        }
    }
}