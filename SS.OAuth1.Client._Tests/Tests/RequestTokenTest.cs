﻿#region

using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth1.Client.Composers;
using SS.OAuth1.Client.Parameters;
using log4net;

#endregion

namespace SS.OAuth1.Client._Tests.Tests
{
    [TestFixture]
    public class RequestTokenTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (RequestTokenTest));
        private readonly Creds _user = G.TestCreds.DanUser;
        private readonly Creds _consumer = G.TestCreds.DanApp;

        [Test]
        public void CallbackPresent_Redirects_ToCallback()
        {
            // Arrange            
            var requestInput = new RequestTokenParameters(_consumer, "http://www.google.com");
            
            // Act
            var requestToken = RequestTokenComposer.GetRequstToken(requestInput);

            // Assert
            Assert.Ignore("Not Implemented");
        }

        [Test, ExpectedException(ExpectedException = typeof (UnauthorizedAccessException))]
        public void BadConsumerKey_Throws_UnauthorizedException()
        {
            // Arrange
            var consumer = new Creds("xxxx", _consumer.Secret);
            var input = new RequestTokenParameters(consumer);

            // Act
            var requestToken = RequestTokenComposer.GetRequstToken(input);

            // Asset
            Assert.Fail("Exception not thrown.");
        }


        [Test, ExpectedException(ExpectedException = typeof (UnauthorizedAccessException))]
        public void BadConsumerSecret_Throws_UnauthorizedException()
        {
            // Arrange
            var consumer = new Creds(_consumer.Key, "bad_secret");
            var input = new RequestTokenParameters(consumer);

            // Act
            var requestToken = RequestTokenComposer.GetRequstToken(input);

            // Assert
            Assert.Fail("Exception not thrown.");
        }


        [Test,
         ExpectedException(ExpectedException = typeof (UnauthorizedAccessException),
             ExpectedMessage = "ErrorCode: InvalidSignature; Message: InvalidSignature")]
        public void BadSignature_Throws_UnauthorizedAccessException_With_InvalidSignatureContent()
        {
            // Arrange
            var consumer = new Creds(_consumer.Key, "dsds");
            var input = new RequestTokenParameters(consumer);

            // Act
            var requestToken = RequestTokenComposer.GetRequstToken(input);

            // Assert
            Assert.Fail("No Exception Thrown");
        }

        [Test]
        public void GetRequestToken_Success()
        {
            // Arrange
            var input = new RequestTokenParameters(_consumer);

            // Act
            var requestToken = RequestTokenComposer.GetRequstToken(input);

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
            var requestToken = RequestTokenComposer.GetRequstToken(input);

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

            // Act
            var msg = parameters.CreateRequestMessage();
            var sender = new MessageSender();
            var response = sender.Send(msg);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        }
    }
}