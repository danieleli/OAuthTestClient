using System.Collections.Specialized;
using SS.OAuth.Extensions;

namespace SS.OAuth.Models.Parameters
{
    public class AccessTokenParams : BaseParams
    {
        public string Verifier { get; set; }
        public string SessionHandle { get; set; }

        public AccessTokenParams(Creds consumer, Creds requestToken, string verifier)
        {
            Consumer = consumer;
            RequestToken = requestToken;
            Verifier = verifier;
        }

        public override string GetSignatureKey()
        {
            return this.Consumer.Secret.UrlEncodeForOAuth() + "&" + RequestToken.Secret.UrlEncodeForOAuth();
        }

        public override NameValueCollection ToCollection()
        {
            var col = base.ToCollection();
            col.AddIfNotNullOrEmpty(OAuth.V1.Keys.VERIFIER, this.Verifier);
            return col;
        }
    }
}