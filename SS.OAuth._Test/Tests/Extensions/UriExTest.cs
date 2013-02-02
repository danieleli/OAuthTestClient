using System;
using NUnit.Framework;
using SS.OAuth.Extensions;
using log4net;

namespace SS.OAuth.Tests.Extensions
{
    [TestFixture]
    public class UriExTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(UriExTest));

        private const string SSL_URL = "HTTPS://WWW.EXAMPLE.COM/ROUT1/ROUT2?NAME=SOMENAME&ZIP=20303";
        private readonly Uri _uri = new Uri(SSL_URL);

        
        [Test]
        public void RfcSampleUri()
        {
            var rfcExample = "http://EXAMPLE.COM:80/r%20v/X?id=123";
            var uri = new Uri(rfcExample);

            var baseUri = uri.GetBaseStringUri();

            LOG.Debug(baseUri);
            Assert.That(baseUri, Is.EqualTo("http://example.com/r%20v/X"), "baseUri");
        }

        [Test]
        public void GetBaseStringUri_Returns_LowerCaseScheme()
        {
            var result = _uri.GetBaseStringUri();

            LOG.Debug("result: " + result);
            Assert.That(result, Is.StringStarting("https"), "scheme");

        }

        [Test]
        public void GetBaseStringUri_Returns_LowerCaseHost()
        {
            var result = _uri.GetBaseStringUri();

            LOG.Debug("result: " + result);
            Assert.That(result, Is.StringContaining("www.example.com"), "host");
        }

        [Test]
        public void GetEncodedBaseUri_Encodes_Slashes()
        {
            Assert.That(_uri.ToString, Contains.Substring("/"), "Slash");

            var result = _uri.GetBaseStringUri().UrlEncodeForOAuth();

            LOG.Debug("result: " + result);
            Assert.That(result, Is.Not.ContainsSubstring("/"), "Slash");
        }


        [Test]
        public void GetEncodedBaseUri_Encodes_Colon()
        {
            Assert.That(_uri.ToString, Contains.Substring(":"), "Colon");

            var result = _uri.GetBaseStringUri().UrlEncodeForOAuth();

            Assert.That(result, Is.Not.ContainsSubstring(":"), "Colon");
        }

    }
}
