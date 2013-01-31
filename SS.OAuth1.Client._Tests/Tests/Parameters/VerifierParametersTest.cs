using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;
using SS.OAuth1.Client._Tests.Tests.Helpers;
using log4net;

namespace SS.OAuth1.Client._Tests.Tests.Parameters
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

            var param = new VerifierTokenParameters(consumer, requestToken);

            // Act
            var header = param.GetOAuthHeader();

            // Assert
            LOG.Info(header);
            Assert.IsNotNullOrEmpty(header, "header");
            Assert.That(header, Contains.Substring(cKey), "consumer key");
            Assert.That(header, Is.Not.ContainsSubstring(cSecret), "consumer secret");
            Assert.That(header, Contains.Substring(rKey), "requestToken key");
            Assert.That(header, Is.Not.ContainsSubstring(rSecret), "requestToken secret");
        }
    }
}
