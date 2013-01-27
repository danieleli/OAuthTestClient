#region

using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1.Tests
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

            // Act
            var requestToken = RequestComposer.RequestTokenHelper.GetRequstToken(consumer, "oob");

            // Asset
            Assert.Fail("Exception not thrown.");
        }


        [Test, ExpectedException(ExpectedException = typeof (UnauthorizedAccessException))]
        public void BadConsumerSecret_Throws_UnauthorizedException()
        {
            // Arrange
            var consumer = new Creds(TestCreds.Dan.Consumer.Key, "xxxx");

            // Act
            var requestToken = RequestComposer.RequestTokenHelper.GetRequstToken(consumer, "oob");

            // Assert
            Assert.Fail("Exception not thrown.");
        }


        [Test,
         ExpectedException(ExpectedException = typeof (UnauthorizedAccessException),
             ExpectedMessage = "ErrorCode: InvalidSignature; Message: InvalidSignature")]
        public void BadSignature_Throws_UnauthorizedAccessException_With_InvalidSignatureContent()
        {
            // Arrange
            var requestMessage = MessageFactory.CreateRequestMessage(OAuth.V1.Routes.REQUEST_TOKEN, HttpMethod.Post);

            var consumer = new Creds(TestCreds.Dan.Consumer.Key, "dsds");
            var input = new RequestTokenParameters(consumer);

            var authHeader = AuthorizationHeaderFactory.CreateRequestTokenHeader(input);
            requestMessage.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);

            // Act
            var response = MessageSender.Send(requestMessage);

            // Assert
            Assert.Fail("No Exception Thrown");
        }

        [Test]
        public void GetRequestToken_Success()
        {
            // Act
            var requestToken = RequestComposer.RequestTokenHelper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Assert
            Assert.IsNotNull(requestToken, "RequestToken");
            Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
            Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Verifier");
        }

        [Test]
        public void GetRequestToken_WithPlusInUserName()
        {
            // Act
            var requestToken = RequestComposer.RequestTokenHelper.GetRequstToken(TestCreds.Dan.User, "oob");

            // Assert
            Assert.IsNotNull(requestToken, "RequestToken");
            Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
            Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Verifier");
        }

        [Test]
        public void MissingAuthHeader_Returns_UnauthorizedStatusCode()
        {
            // Act
            var msg = MessageFactory.CreateRequestMessage(OAuth.V1.Routes.REQUEST_TOKEN, HttpMethod.Post);
            var response = MessageSender.Send(msg);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}