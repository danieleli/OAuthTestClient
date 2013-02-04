﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth.Extensions;
using SS.OAuth.Factories;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;
using log4net;

namespace SS.OAuth.Tests
{
    public class RfcParameters : BaseParams
    {
        private readonly string _testNonce;
        private readonly string _testTimestamp;

        public RfcParameters(Creds consumer, Creds requestToken, string nonce, string timestamp)
        {
            Consumer = consumer;
            RequestToken = requestToken;
            _testTimestamp = timestamp;
            _testNonce = nonce;
        }

        public override string Nonce
        {
            get { return _testNonce; }
        }

        public override string Timestamp
        {
            get { return _testTimestamp; }
        }

        public override NameValueCollection ToCollection()
        {
            var col = new NameValueCollection();
            col.Add(OAuth.V1.Keys.NONCE, this.Nonce);
            col.Add(OAuth.V1.Keys.TIMESTAMP, this.Timestamp);
            col.Add(OAuth.V1.Keys.SIGNATURE_METHOD, OAuth.V1.Values.SIGNATURE_METHOD);
            col.Add(OAuth.V1.Keys.CONSUMER_KEY, this.Consumer.Key);
          
            // No version in rfc sample so don't add.
            // col.Add(OAuth.V1.Keys.VERSION, OAuth.V1.Values.VERSION);

            col.AddIfNotNullOrEmpty(OAuth.V1.Keys.REALM, this.Realm);
            if (this.RequestToken != null)
            {
                col.AddIfNotNullOrEmpty(OAuth.V1.Keys.TOKEN, this.RequestToken.Key);
            }

            return col;
        }
    }

    [TestFixture]
    public class RfcSample
    {
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

        private static readonly ILog LOG = LogManager.GetLogger(typeof(RfcSample));

        const string URL = "HTTP://example.com/request?b5=%3D%253D&a3=a&c%40=&a2=r%20b";
        const string NONCE = "7d8f3e4a";
        const string TIMESTAMP = "137131201";

        const string C_KEY = "9djdj82h48djs9d2";
        const string C_SECRET = "consumerSecret";
        readonly Creds _consumer = new Creds(C_KEY, C_SECRET);

        const string R_KEY = "kkk9d7dh3k39sjv7";
        const string R_SECRET = "tokenSecret";
        readonly Creds _requestToken = new Creds(R_KEY, R_SECRET);

        readonly HttpMethod _method = HttpMethod.Post;

        readonly RfcParameters _paramz;
        private readonly HttpRequestMessage _msg;


        // Constructor
        public RfcSample()
        {
            _paramz = new RfcParameters(_consumer, _requestToken, NONCE, TIMESTAMP);
            _msg = new HttpRequestMessage(_method, URL);

            var contentDictionary = new Dictionary<string, string> { { "c2", null }, { "a3", "2 q" } };
            _msg.Content = new FormUrlEncodedContent(contentDictionary);
        }

        [Test]
        public void SignatureBase()
        {
            // Arrange 
            var sigFactory = new SignatureFactory(_paramz);

            // Act
            var sigBase = sigFactory.SignatureBaseStringFactory.GetSignatureBase(_msg);

            // Assert
            LOG.Debug(sigBase);
            Assert.That(sigBase, Is.EqualTo(SIGNATURE_BASE));
        }

        [Test]
        public void Normalize()
        {
            // Arrange 
            var sigFactory = new SignatureFactory(_paramz);
            
            // Act
            var normalizedRequestParams = sigFactory.SignatureBaseStringFactory.GetAllRequestParameters(_msg);
            var normalized = normalizedRequestParams.Normalize();

            // Assert
            LOG.Debug(normalized);
            Assert.That(normalized, Is.EqualTo(NORMALIZED_REQUEST_PARAMETERS));
        }
    }
}
