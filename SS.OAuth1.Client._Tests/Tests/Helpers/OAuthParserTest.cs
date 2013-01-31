using System;
using NUnit.Framework;
using SS.OAuth1.Client.Extensions;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client._Tests.Tests.Helpers
{
    [TestFixture]
    public class OAuthParserTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(OAuthParserTest));

        [Test]
        public void CreateHeader_Contains_ConsumerKey()
        {
            // Arrange
            const string key = "keyABC";
            const string secret = "secretDEF";
            var consumer = new Creds(key, secret);
            var param = new RequestTokenParameters(consumer);
            var parser = new OAuthParser();

            // Act
            var header = parser.CreateHeader(param, null);

            // Assert
            LOG.Info(header);
            Assert.IsNotNullOrEmpty(header, "header");
            Assert.That(header, Contains.Substring(key), "consumer key");
            Assert.That(header, Is.Not.ContainsSubstring(secret), "consumer secret");
        }

        private const string SIMPLE_URL = "HTTP://WWW.EXAMPLE.COM/ROUT1/ROUT2?NAME=SOMENAME&ZIP=20303";
        private const string SSL_URL = "HTTPS://WWW.EXAMPLE.COM/ROUT1/ROUT2?NAME=SOMENAME&ZIP=20303";
        
        [Test]
        public void GetBaseStringUri_Returns_LowerCaseScheme()
        {

            var uri = new Uri(SSL_URL);

            var result = UriEx.GetBaseStringUri(uri);


            LOG.Debug("result: " + result);
            Assert.That(result, Is.StringStarting("https"), "scheme");

        }

        [Test]
        public void GetBaseStringUri_Returns_LowerCaseHost()
        {

            var uri = new Uri(SSL_URL);

            var result = UriEx.GetBaseStringUri(uri);


            LOG.Debug("result: " + result);
            Assert.That(result, Is.StringEnding("www.example.com"), "host");

        }

    }
}
