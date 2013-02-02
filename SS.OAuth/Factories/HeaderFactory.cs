using System.Collections.Specialized;
using SS.OAuth.Extensions;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Factories
{
    public class HeaderFactory
    {
        public NameValueCollection GetOAuthParams(BaseParams p)
        {
            var col = new NameValueCollection();
            col.Add(OAuth.V1.Keys.NONCE, p.Nonce);
            col.Add(OAuth.V1.Keys.TIMESTAMP, p.Timestamp);
            col.Add(OAuth.V1.Keys.SIGNATURE_METHOD, OAuth.V1.Values.SIGNATURE_METHOD);
            col.Add(OAuth.V1.Keys.CONSUMER_KEY, p.Consumer.Key);

            col.AddIfNotNullOrEmpty(OAuth.V1.Keys.REALM, p.Realm);
            if (p.RequestToken != null)
            {
                col.AddIfNotNullOrEmpty(OAuth.V1.Keys.TOKEN, p.RequestToken.Key);
            }

            return col;
        }

        public NameValueCollection GetOAuthParams(RequestTokenParams p)
        {
            var col = GetOAuthParams((BaseParams) p);
            col.AddIfNotNullOrEmpty(OAuth.V1.Keys.CALLBACK, p.Callback);
            return col;
        }

        public NameValueCollection GetOAuthParams(AccessTokenParams p)
        {
            var col = GetOAuthParams((BaseParams)p);
            col.AddIfNotNullOrEmpty(OAuth.V1.Keys.VERIFIER, p.Verifier);
            return col;
        }
    }
}
