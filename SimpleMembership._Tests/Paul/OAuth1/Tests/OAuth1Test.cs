#region

using System;
using System.Net;
using MXM.API.Test.Controllers;
using NUnit.Framework;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1.Tests
{
    [TestFixture]
    public class OAuth1Test
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(OAuth1Test));

        public static class Helper
        {
            public static Creds GetThreeLegAccessToken(Creds consumer, Creds user, string returnUrl)
            {
                var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(consumer, returnUrl);
                var verifier = OAuth1Helper.VerifierTokenHelper.GetVerifierToken(requestToken, consumer, user);
                var accessToken = OAuth1Helper.AccessTokenHelper.GetAccessToken(consumer, verifier);

                return accessToken;
            }

            public static void GetTwoLegAccessToken(Creds user, string returnUrl)
            {
                GetThreeLegAccessToken(user, user, returnUrl);
            }
        }



        #region -- Verifier --


        [Test]
        public void Authorize_Returns_StatusCodeOk()
        {
            // Arrange
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Act
            var response = OAuth1Helper.VerifierTokenHelper.GetAuthorizeResponse(requestToken);
                                       
            // Assert
            LOG.Debug("Response: " + response);
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK),"HttpStatusCode");
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

        #endregion  -- Verifier --

        [Test] // Three leg has different consumer and user creds.
        public void ThreeLegged_Success()
        {
            // Act
            var accessToken = Helper.GetThreeLegAccessToken(TestCreds.Dan.Consumer, TestCreds.Dan.User, "oob");

            // Assert
            Assert.IsNotNull(accessToken, "AccessToken");
            Assert.IsNotNullOrEmpty(accessToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(accessToken.Secret, "oauth_token_secret");
        }

        [Test] // Two leg is same as three leg but reuse consumer creds as user creds.
        public void TwoLegged_Success()
        {
            // Act            
            var accessToken = Helper.GetThreeLegAccessToken(TestCreds.TwoLegUser, TestCreds.TwoLegUser, "oob");

            // Assert
            Assert.IsNotNull(accessToken, "AccessToken");
            Assert.IsNotNullOrEmpty(accessToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(accessToken.Secret, "oauth_token_secret");
        }

    }
}