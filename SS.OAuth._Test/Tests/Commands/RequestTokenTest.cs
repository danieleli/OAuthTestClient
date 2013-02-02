#region

using System;
using System.Net;
using NUnit.Framework;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Messages;
using SS.OAuth1.Client.Parameters;
using log4net;

#endregion

namespace SS.OAuth1.Client._Tests.Tests.GetTokenCommand
{
    [TestFixture]
    public class RequestTokenTest
    {
        private const string INVALID_SIGNATURE_MESSAGE = "ErrorCode: InvalidSignature; Message: Invalid signature.";
        private static readonly ILog LOG = LogManager.GetLogger(typeof(RequestTokenTest));
        private readonly Creds _user = G.TestCreds.DanUser;
        private readonly Creds _consumer = G.TestCreds.DanConsumer;
        Commands.GetTokenCommand _cmd = new Commands.GetTokenCommand();

        [Test]
        public void CallbackPresent_Redirects_ToCallback()
        {
            // Arrange            
            var requestInput = new RequestTokenParameters(_consumer, "http://www.google.com");

            // Act
            var requestToken = _cmd.GetToken(requestInput);

            // Assert
            Assert.Ignore("Not Implemented");
        }

        [Test, ExpectedException(ExpectedException = typeof(UnauthorizedAccessException))]
        public void BadConsumerKey_Throws_UnauthorizedException()
        {
            // Arrange
            var consumer = new Creds("xxxx", _consumer.Secret);
            var input = new RequestTokenParameters(consumer);

            // Act
            var requestToken = _cmd.GetToken(input);

            // Asset
            Assert.Fail("Exception not thrown.");
        }


        [Test, ExpectedException(ExpectedException = typeof(UnauthorizedAccessException))]
        public void BadConsumerSecret_Throws_UnauthorizedException()
        {
            // Arrange
            var consumer = new Creds(_consumer.Key, "bad_secret");
            var input = new RequestTokenParameters(consumer);

            // Act
            var requestToken = _cmd.GetToken(input);

            // Assert
            Assert.Fail("Exception not thrown.");
        }


        [Test,
            ExpectedException(ExpectedException = typeof(UnauthorizedAccessException),
                ExpectedMessage = INVALID_SIGNATURE_MESSAGE)]
        public void BadSignature_Throws_UnauthorizedAccessException_With_InvalidSignatureContent()
        {
            // Arrange
            var consumer = new Creds(_consumer.Key, "dsds");
            var input = new RequestTokenParameters(consumer);

            // Act
            var requestToken = _cmd.GetToken(input);

            // Assert
            Assert.Fail("No Exception Thrown");
        }

        [Test]
        public void GetRequestToken_Success()
        {
            // Arrange
            var input = new RequestTokenParameters(_consumer);

            // Act
            var requestToken = _cmd.GetToken(input);

            // Assert
            Assert.IsNotNull(requestToken, "RequestToken");
            Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
            Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Secret");
        }

        [Test]
        public void GetRequestToken_WithPlusInUserName()
        {
            // Arrange
            var input = new RequestTokenParameters(_user);

            // Act
            var requestToken = _cmd.GetToken(input);

            // Assert
            Assert.IsNotNull(requestToken, "RequestToken");
            Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
            Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Secret");
        }

        [Test]
        public void MissingAuthHeader_Returns_UnauthorizedStatusCode()
        {
            // Arrange
            var parameters = new RequestTokenParameters(_consumer);
            var p = new Commands.GetTokenCommand();
            var msg = p.CreateMessage(parameters);
            var sender = new MessageSender();

            // Act
            var response = sender.Send(msg);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }

        [Test]
        public void CreateHeader_Contains_ConsumerKey()
        {
            // Arrange
            const string key = "keyABC";
            const string secret = "secretDEF";
            var consumer = new Creds(key, secret);
            var param = new RequestTokenParameters(consumer);

            // Act
            var header = param.GetOAuthHeader();

            // Assert
            LOG.Info(header);
            Assert.IsNotNullOrEmpty(header, "header");
            Assert.That(header, Contains.Substring(key), "consumer key");
            Assert.That(header, Is.Not.ContainsSubstring(secret), "consumer secret");
        }
    }
}