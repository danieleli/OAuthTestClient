using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SS.OAuth1.Client.Extensions;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client._Tests.Tests.Helpers
{
    [TestFixture]
    public class RfcSample
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(RfcSample));

        /*
            POST /request?b5=%3D%253D&a3=a&c%40=&a2=r%20b HTTP/1.1
            Host: example.com
            Content-Type: application/x-www-form-urlencoded
            Authorization: OAuth realm="Example",
                            oauth_consumer_key="9djdj82h48djs9d2",
                            oauth_token="kkk9d7dh3k39sjv7",
                            oauth_signature_method="HMAC-SHA1",
                            oauth_timestamp="137131201",
                            oauth_nonce="7d8f3e4a",
                            oauth_signature="djosJKDKJSD8743243%2Fjdk33klY%3D"

            c2&a3=2+q
         */

        public class RfcParameters   : OAuthParametersBase
        {
            private readonly string _testNonce;
            private readonly string _testTimestamp;
            
            public Creds RequestToken { get; set; }

            public RfcParameters(Creds consumer, Creds requestToken, HttpMethod method, string url, string nonce, string timestamp, string authority = null)
                : base(consumer, method, url, authority, false)
            {
                RequestToken = requestToken;
                _testTimestamp = timestamp;
                _testNonce = nonce;
            }

            public override NameValueCollection GetOAuthParams()
            {
                var paramPairs = base.GetOAuthParamsCore();
                paramPairs.AddIfNotNullOrEmpty(Keys.TOKEN, this.RequestToken.Key);
                
                return paramPairs;
            }

            public override string Nonce
            {
                get
                {
                    return _testNonce;
                }
            }

            public override string Timestamp
            {
                get
                {
                    return _testTimestamp;
                }
            }

            public override string GetOAuthHeader()
            {
                throw new NotImplementedException();
            }
        }

        [Test]
        public void RfcSample_Good()
        {
            // Arrange
            const string url = "HTTP://example.com/request?b5=%3D%253D&a3=a&c%40=&a2=r%20b";
            const string authority = "authority";
            const string nonce = "7d8f3e4a";
            const string timestamp = "137131201";

            var method = HttpMethod.Get;

            const string cKey = "9djdj82h48djs9d2";
            const string cSecret = "consumerSecret";
            var consumer = new Creds(cKey, cSecret);
            
            const string rKey = "kkk9d7dh3k39sjv7";
            const string rSecret = "tokenSecret";
            var requestToken = new Creds(rKey, rSecret);

            var content = new NameValueCollection{{"c2",null}, {"a3","2 q"}};
            
            var p = new RfcParameters(consumer, requestToken, method, url, nonce, timestamp, authority);

            var normalizedRequestParams = p.GetAllRequestParameters(content);
            var normalized = normalizedRequestParams.Normalize();
            LOG.Debug(normalized);
            Assert.That(normalized, Is.EqualTo(EXPECTED));

        }

        private const string EXPECTED = "a2=r%20b&a3=2%20q&a3=a&b5=%3D%253D&c%40=&c2=&oauth_consumer_key=9dj" +
                                        "dj82h48djs9d2&oauth_nonce=7d8f3e4a&oauth_signature_method=HMAC-SHA1" +
                                        "&oauth_timestamp=137131201&oauth_token=kkk9d7dh3k39sjv7";


    }
}
