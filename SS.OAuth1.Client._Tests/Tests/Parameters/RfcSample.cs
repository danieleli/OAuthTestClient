using System;
using System.Collections.Specialized;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth1.Client.Extensions;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client._Tests.Tests.Parameters
{
    public class RfcParameters : OAuthParametersBase
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
            paramPairs.AddIfNotNullOrEmpty(OAuth.V1.Keys.TOKEN, this.RequestToken.Key);

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

    [TestFixture]
    public class RfcSample
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(RfcSample));
        private RfcParameters _paramz;
        private NameValueCollection _content;

        private const string NORMALIZED_REQUEST_PARAMETERS =
            "a2=r%20b&a3=2%20q&a3=a&b5=%3D%253D&c%40=&c2=&oauth_consumer_key=9dj" +
            "dj82h48djs9d2&oauth_nonce=7d8f3e4a&oauth_signature_method=HMAC-SHA1" +
            "&oauth_timestamp=137131201&oauth_token=kkk9d7dh3k39sjv7";

        private const string SIGNATURE_BASE =
            "POST&http%3A%2F%2Fexample.com%2Frequest&a2%3Dr%2520b%26a3%3D2%2520q" +
            "%26a3%3Da%26b5%3D%253D%25253D%26c%2540%3D%26c2%3D%26oauth_consumer_" +
            "key%3D9djdj82h48djs9d2%26oauth_nonce%3D7d8f3e4a%26oauth_signature_m" +
            "ethod%3DHMAC-SHA1%26oauth_timestamp%3D137131201%26oauth_token%3Dkkk" +
            "9d7dh3k39sjv7";

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

        public RfcSample()
        {
            // Arrange
            const string url = "HTTP://example.com/request?b5=%3D%253D&a3=a&c%40=&a2=r%20b";
            const string authority = "authority";
            const string nonce = "7d8f3e4a";
            const string timestamp = "137131201";

            var method = HttpMethod.Post;

            const string cKey = "9djdj82h48djs9d2";
            const string cSecret = "consumerSecret";
            var consumer = new Creds(cKey, cSecret);

            const string rKey = "kkk9d7dh3k39sjv7";
            const string rSecret = "tokenSecret";
            var requestToken = new Creds(rKey, rSecret);

            _content = new NameValueCollection { { "c2", null }, { "a3", "2 q" } };

            _paramz = new RfcParameters(consumer, requestToken, method, url, nonce, timestamp, authority);   
        }

        [Test]
        public void SignatureBase()
        {
            // Act
            var sigBase = _paramz.GetSignatureBase(_content);
            
            // Assert
            LOG.Debug(sigBase);
            Assert.That(sigBase, Is.EqualTo(SIGNATURE_BASE));
        }

        [Test]
        public void Normalize()
        {
            // Act
            var normalizedRequestParams = _paramz.GetAllRequestParameters(_content);
            var normalized = normalizedRequestParams.Normalize();
            
            // Assert
            LOG.Debug(normalized);
            Assert.That(normalized, Is.EqualTo(NORMALIZED_REQUEST_PARAMETERS));
        }
    }
}
