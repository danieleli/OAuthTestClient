#region

using System.Collections.Generic;
using System.Net.Http;

#endregion

namespace SS.OAuth1.Client.Parameters
{
    public class AccessTokenParameters : OAuthParametersBase
    {
        public Creds Token { get; set; }
        public string RequestTokenSecret { get; set; }
        public string SessionHandle { get; set; }

        #region -- Constructor --

        public AccessTokenParameters(Creds consumer, Creds token)
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.ACCESS_TOKEN)
        {
            NullCheck(token, "token");
            NullCheck(token.Key, "requestToken");
            NullCheck(token.Secret, "requestTokenSecret");

            Token = token;
        }

        #endregion -- Constructor --

        public override string GetAuthHeader()
        {
            var signature = Signature.GetOAuth1ASignature(this.RequestUri,
                                              this.HttpMethod,
                                              this.Consumer.Key,
                                              this.Consumer.Secret,
                                              this.Token.Key,
                                              this.Token.Secret,
                                              this.Timestamp,
                                              this.Nonce,
                                              null,
                                              null);

            var oauthParams = AuthParameterFactory.GetOAuthParams(this.Consumer.Key,
                                                                  this.Nonce,
                                                                  signature,
                                                                  this.Timestamp, 
                                                                  null,
                                                                  this.Token.Key);


            var header = "OAuth " + oauthParams.Stringify();

            return header;

        }


        private SortedDictionary<string, string> ToSortedDictionary()
        {
            var d = new SortedDictionary<string, string>();

            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.CONSUMER_KEY, this.Consumer.Key);
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.NONCE, this.Nonce);
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.SIGNATURE_METHOD, SIGNATURE_METHOD);
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.TIMESTAMP, this.Timestamp);
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.VERSION, VERSION);
            d.AddIfNotNullOrEmpty(AuthParameterFactory.Keys.TOKEN, this.Token.Key);

            return d;
        }
    }
}