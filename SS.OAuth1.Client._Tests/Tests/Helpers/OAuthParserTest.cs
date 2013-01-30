using NUnit.Framework;
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
            const string key ="keyABC";
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
    }
}
