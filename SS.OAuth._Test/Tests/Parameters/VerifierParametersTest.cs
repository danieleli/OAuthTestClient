using NUnit.Framework;
using SS.OAuth.Extensions;
using SS.OAuth.Factories;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;
using log4net;

namespace SS.OAuth.Tests.Parameters
{
    [TestFixture]
    public class VerifierParametersTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(VerifierParametersTest));

        [Test]
        public void CreateHeader_Contains_ConsumerKeyAndRequestToken()
        {
            // Arrange
            const string cKey = "consumerKey";
            const string cSecret = "consumerSecret";
            var consumer = new Creds(cKey, cSecret);

            const string rKey = "requestTokenKey";
            const string rSecret = "requestTokenSecret";
            var requestToken = new Creds(rKey, rSecret);

            var param = new VerifierTokenParams(consumer, requestToken, "sss");
            var headerFactory = new HeaderFactory();

            // Act
            var header = headerFactory.GetOAuthParams(param).Stringify();

            // Assert
            LOG.Info(header);
            Assert.That(header, Is.Not.Null, "header");
            Assert.That(header, Contains.Substring(cKey), "consumer key");
            Assert.That(header, Is.Not.ContainsSubstring(cSecret), "consumer secret");
            Assert.That(header, Contains.Substring(rKey), "requestToken key");
            Assert.That(header, Is.Not.ContainsSubstring(rSecret), "requestToken secret");
        }
    }
}
