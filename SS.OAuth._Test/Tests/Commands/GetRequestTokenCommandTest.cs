using NUnit.Framework;
using SS.OAuth.Commands;
using SS.OAuth.Models.Parameters;
using log4net;

namespace SS.OAuth.Tests.Commands
{

    [TestFixture]
    public class GetRequestTokenCommandTest
    {
        private static readonly ILog LOG  = LogManager.GetLogger(typeof(GetRequestTokenCommandTest));

        [Test]
        public void HappyPath()
        {
            // Arrange            
            var requestParam = new RequestTokenParams(G.DanTestAppConsumer);
            var cmd = new GetRequestTokenCommand(requestParam);

            // Act
            var token = cmd.GetToken();

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token.Key, Is.Not.Null, "Key");
            Assert.That(token.Key, Is.Not.Empty, "Key");
            Assert.That(token.Key, Is.Not.Null, "Secret");
            Assert.That(token.Key, Is.Not.Empty, "Secret");
        }
    }
}
