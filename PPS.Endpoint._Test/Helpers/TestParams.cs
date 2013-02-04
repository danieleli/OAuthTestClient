using System.Collections.Specialized;
using SS.OAuth;
using SS.OAuth.Extensions;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace PPS.Endpoint.Helpers
{
    public class TestTokenParams : BaseParams
    {
        private readonly bool _includeVersion;
        private readonly string _nonce;
        private readonly string _timestamp;

        private bool IncludeVersion { get; set; }

        public TestTokenParams( Creds consumer, string nonce = null, string timestamp = null, bool includeVersion = false )
        {
            IncludeVersion = includeVersion;
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
            return base.ToCollectionInternal();
        }
    }
}