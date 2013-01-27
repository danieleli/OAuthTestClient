#region

using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using log4net;

#endregion

namespace SS.OAuth1.Client._Tests.Tests
{
    [TestFixture]
    public class RequestTokenTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (RequestTokenTest));


        [Test, ExpectedException(ExpectedException = typeof (UnauthorizedAccessException))]
        public void BadConsumerKey_Throws_UnauthorizedException()
        {
            // Arrange
            var consumer = new Creds("xxxx", TestCreds.Dan.Consumer.Secret);
            var input = new RequestTokenParameters(consumer);

            // Act
            var requestToken = RequestComposer.RequestTokenHelper.GetRequstToken(input);

            // Asset
            Assert.Fail("Exception not thrown.");
        }


        [Test, ExpectedException(ExpectedException = typeof (UnauthorizedAccessException))]
        public void BadConsumerSecret_Throws_UnauthorizedException()
        {
            // Arrange
            var consumer = new Creds(TestCreds.Dan.Consumer.Key, "bad_secret");
            var input = new RequestTokenParameters(consumer);

            // Act
            var requestToken = RequestComposer.RequestTokenHelper.GetRequstToken(input);

            // Assert
            Assert.Fail("Exception not thrown.");
        }


        [Test,
         ExpectedException(ExpectedException = typeof (UnauthorizedAccessException),
             ExpectedMessage = "ErrorCode: InvalidSignature; Message: InvalidSignature")]
        public void BadSignature_Throws_UnauthorizedAccessException_With_InvalidSignatureContent()
        {
            // Arrange
            var consumer = new Creds(TestCreds.Dan.Consumer.Key, "dsds");
            var input = new RequestTokenParameters(consumer);

            // Act
            var requestToken = RequestComposer.RequestTokenHelper.GetRequstToken(input);

            // Assert
            Assert.Fail("No Exception Thrown");
        }

        [Test]
        public void GetRequestToken_Success()
        {
            // Arrange
            var input = new RequestTokenParameters(TestCreds.Dan.Consumer);

            // Act
            var requestToken = RequestComposer.RequestTokenHelper.GetRequstToken(input);

            // Assert
            Assert.IsNotNull(requestToken, "RequestToken");
            Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
            Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Verifier");
        }

        [Test]
        public void GetRequestToken_WithPlusInUserName()
        {
            // Arrange
            var input = new RequestTokenParameters(TestCreds.Dan.User);

            // Act
            var requestToken = RequestComposer.RequestTokenHelper.GetRequstToken(input);

            // Assert
            Assert.IsNotNull(requestToken, "RequestToken");
            Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
            Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Verifier");
        }

        [Test]
        public void MissingAuthHeader_Returns_UnauthorizedStatusCode()
        {
            // Arrange
            var input = new RequestTokenParameters(TestCreds.Dan.Consumer);

            // Act
            var msg = MessageFactory.CreateRequestMessage(input);
            var response = MessageSender.Send(msg);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}