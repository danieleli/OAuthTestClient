using System;
using System.Net;
using System.Net.Http;
using MXM.API.Test.Controllers;
using NUnit.Framework;
using log4net;

namespace SimpleMembership._Tests.Paul.OAuth1.Tests
{
    [TestFixture]
    public class GetRequestTokenTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(GetRequestTokenTest));

        [Test]
        public void GetRequestToken_Success()
        {
            // Act
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Assert
            Assert.IsNotNull(requestToken, "RequestToken");
            Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
            Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Token");
        }

        [Test]
        public void MissingAuthHeader_Returns_UnauthorizedStatusCode()
        {
            // Act
            var msg = MsgHelper.CreateRequestMessage(OAuth.V1.Routes.REQUEST_TOKEN, HttpMethod.Post);
            var response = MsgHelper.Send(msg);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }


        [Test, ExpectedException(ExpectedException = typeof(UnauthorizedAccessException))]
        public void BadConsumerKey_Throws_UnauthorizedException()
        {
            // Arrange
            var consumer = new Creds("xxxx", TestCreds.Dan.Consumer.Secret);

            // Act
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(consumer, "oob");

            // Asset
            Assert.Fail("Exception not thrown.");
        }


        [Test, ExpectedException(ExpectedException = typeof(UnauthorizedAccessException))]
        public void BadConsumerSecret_Throws_UnauthorizedException()
        {
            // Arrange
            var consumer = new Creds(TestCreds.Dan.Consumer.Key, "xxxx");
            
            // Act
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(consumer, "oob");

            // Assert
            Assert.Fail("Exception not thrown.");
        }


        [Test, ExpectedException(ExpectedException = typeof(UnauthorizedAccessException), ExpectedMessage = "ErrorCode: InvalidSignature; Message: InvalidSignature")]
        public void BadSignature_Throws_UnauthorizedAccessException_With_InvalidSignatureContent()
        {
            // Arrange
            var requestMessage = MsgHelper.CreateRequestMessage(OAuth.V1.Routes.REQUEST_TOKEN, HttpMethod.Post);

            var badSignature = "xxxx";
            var timestamp = OAuthUtils.GenerateTimeStamp();
            var nonce = OAuthUtils.GenerateNonce();

            var authHeader = Crypto.GetAuthHeader(TestCreds.Dan.Consumer, "oob", nonce, timestamp, badSignature);
            requestMessage.Headers.Add(Crypto.AUTHORIZATION_HEADER, authHeader);

            // Act
            var response = MsgHelper.Send(requestMessage);

            // Assert
            Assert.Fail("No Exception Thrown");
        }

    }
}