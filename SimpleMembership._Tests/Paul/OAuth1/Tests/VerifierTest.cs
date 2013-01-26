﻿using System;
using System.Net;
using NUnit.Framework;
using log4net;

namespace SimpleMembership._Tests.Paul.OAuth1.Tests
{
    [TestFixture]
    public class VerifierTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(VerifierTest));

        [Test]
        public void Authorize_Returns_StatusCodeOk()
        {
            // Arrange
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Act
            var response = OAuth1Helper.VerifierTokenHelper.GetAuthorizeResponse(requestToken);

            // Assert
            LOG.Debug("Response: " + response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HttpStatusCode");
        }

        [Test]
        public void AuthorizeWithInvalidToken_Returns_BadRequest()
        {
            // Arrange
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(TestCreds.Dan.Consumer, "oob");
            requestToken.Key = "xxxx";

            // Act
            var response = OAuth1Helper.VerifierTokenHelper.GetAuthorizeResponse(requestToken);

            // Assert
            LOG.Debug("Response: " + response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "HttpStatusCode");
        }

        [Test]
        public void When_ActiveMxMerchantUserSession_AuthorizeToken_Displays_ApproveDenyView()
        {
            Assert.Ignore("Not Implemented");
        }

        [Test]
        public void When_NoActiveUserSessionOnMxMerchant_AuthorizeToken_RedirectsToLogin()
        {
            // Arrange
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Act
            var response = OAuth1Helper.VerifierTokenHelper
                                       .GetAuthorizeResponse(requestToken);


            // Assert
            LOG.Debug("Response: " + response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect), "Response.StatusCode");
            Assert.That(response.Headers.Location, Contains.Substring("login"));
        }

        [Test]
        public void GetAccessToken_By_SimulatedUserLogin()
        {
            var passwordHash = "5ravvW12u10gQVQtfS4/rFuwVZM=";
            var user = new Creds("dantest", passwordHash);
            var accessToken = OAuth1Helper.UserTokenHelper.GetUserToken(user);
        }


        [Test]
        public void GetVerifier()
        {
            // Arrange
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Act
            var verifier = OAuth1Helper.VerifierTokenHelper
                                       .GetVerifierToken(requestToken, TestCreds.Dan.Consumer, TestCreds.Dan.User);

            // Assert
            LOG.Debug("Verifier: " + verifier);
            Assert.IsNotNullOrEmpty(verifier.Secret, "Verifier");
        }

        [Test, ExpectedException(ExpectedException = typeof(UnauthorizedAccessException))]
        public void BadVerifier_Returns_UnauthorizedStatusCode()
        {
            // Arrrange
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Act

            // Assert
            Assert.Ignore("Not Implemented");
        }

        
    }
}