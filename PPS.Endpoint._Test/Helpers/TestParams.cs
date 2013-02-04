using System.Collections.Specialized;
using SS.OAuth.Extensions;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Helpers
{
    public class TestParams : BaseParams
    {
        private readonly string _nonce;
        private readonly string _timestamp;

        public TestParams(Creds consumer, string nonce, string timestamp)
        {
            _nonce = nonce;
            _timestamp = timestamp;

            Consumer = consumer;
        }

        public override string Timestamp
        {
            get { return _timestamp; }
        }

        public override string Nonce
        {
            get { return _nonce; }
        }


        public override NameValueCollection ToCollection()
        {
            var col = new NameValueCollection
                {
                    {OAuth.V1.Keys.NONCE, this.Nonce},
                    {OAuth.V1.Keys.TIMESTAMP, this.Timestamp},
                    {OAuth.V1.Keys.SIGNATURE_METHOD, OAuth.V1.Values.SIGNATURE_METHOD},
                    {OAuth.V1.Keys.CONSUMER_KEY, this.Consumer.Key}
                };
            // exclude version to match test expectations
            // col.Add(OAuth.V1.Keys.VERSION, OAuth.V1.Values.VERSION);

            col.AddIfNotNullOrEmpty(OAuth.V1.Keys.REALM, this.Realm);
            if (this.RequestToken != null)
            {
                col.AddIfNotNullOrEmpty(OAuth.V1.Keys.TOKEN, this.RequestToken.Key);
            }

            return col;
        }
    }
}