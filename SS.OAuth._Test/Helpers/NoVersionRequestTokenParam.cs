using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS.OAuth.Extensions;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Helpers
{
    public class NoVersionRequestTokenParams : RequestTokenParams
    {
        public NoVersionRequestTokenParams(Creds consumer, string callback = "oob")
            : base(consumer, callback)
        {
        }

        public override NameValueCollection ToCollection()
        {
            var col = new NameValueCollection();
            col.Add(OAuth.V1.Keys.NONCE, this.Nonce);
            col.Add(OAuth.V1.Keys.TIMESTAMP, this.Timestamp);
            col.Add(OAuth.V1.Keys.SIGNATURE_METHOD, OAuth.V1.Values.SIGNATURE_METHOD);
            col.Add(OAuth.V1.Keys.CONSUMER_KEY, this.Consumer.Key);
            // col.Add(OAuth.V1.Keys.VERSION, OAuth.V1.Values.VERSION);

            col.AddIfNotNullOrEmpty(OAuth.V1.Keys.REALM, this.Realm);
            if (this.RequestToken != null)
            {
                col.AddIfNotNullOrEmpty(OAuth.V1.Keys.TOKEN, this.RequestToken.Key);
            }

            col.AddIfNotNullOrEmpty(OAuth.V1.Keys.CALLBACK, this.Callback);
            return col;

        }
    }

}
