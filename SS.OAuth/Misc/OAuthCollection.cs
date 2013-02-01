using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Misc
{
    public sealed class OAuthCollection: NameValueCollection
    {
        public void AddVersion()
        {
            this.Add(OAuth.V1.Keys.VERSION, OAuth.V1.Values.VERSION);
        }
        public OAuthCollection(BaseParameters p)
        {
            
            this.Add(OAuth.V1.Keys.NONCE, p.Nonce);
            this.Add(OAuth.V1.Keys.TIMESTAMP, p.Timestamp);
            this.Add(OAuth.V1.Keys.SIGNATURE_METHOD, OAuth.V1.Values.SIGNATURE_METHOD);
            this.Add(OAuth.V1.Keys.CONSUMER_KEY, p.Consumer.Key);
            
            this.AddIfNotNullOrEmpty(OAuth.V1.Keys.REALM, p.Realm);
        }


        public OAuthCollection(RequestTokenParameters p)
            : this((BaseParameters)p)
        {
            this.AddIfNotNullOrEmpty(OAuth.V1.Keys.CALLBACK, p.Callback);
        }

        public OAuthCollection(VerifierTokenParameters p)
            : this((BaseParameters)p)
        {
            this.AddIfNotNullOrEmpty(OAuth.V1.Keys.TOKEN, p.RequestToken.Key);
        }


        public OAuthCollection(AccessTokenParameters p)
            : this((BaseParameters)p)
        {
            this.AddIfNotNullOrEmpty(OAuth.V1.Keys.TOKEN, p.RequestToken.Key);
            this.AddIfNotNullOrEmpty(OAuth.V1.Keys.VERIFIER, p.Verifier);
        }
    }
}
