using System.Collections.Specialized;
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
    }
}