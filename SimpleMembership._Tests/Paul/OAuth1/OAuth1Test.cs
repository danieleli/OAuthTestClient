#region

using System.Runtime.Remoting;
using MXM.API.Test.Controllers;
using NUnit.Framework;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1
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


        #region -- Request --

        [Test]
        public void GetRequestToken()
        {
            // Act
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Assert
            Assert.IsNotNull(requestToken, "RequestToken");
            Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
            Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Token");
        }

        [Test,
         ExpectedException(ExpectedException = typeof(ServerException),
             ExpectedMessage = "something like: Auth header missing")]
        public void MissingAuthHeader_Should_ThrowServerException_On_RequestToken()
        {
            // Act
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Assert
            Assert.Fail("Expected ServerException not thrown.");
        }

        #endregion -- Request --

        #region -- Verifier --

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

        [Test, ExpectedException(ExpectedException = typeof(ServerException))]
        public void BadVerifier_Should_ThrowServerException_On_RequestVerifier()
        {
            // Arrrange
            var requestToken = OAuth1Helper.RequestTokenHelper.GetRequstToken(TestCreds.ThreeLegConsumer, "oob");

            // Act
            var verifier = OAuth1Helper.VerifierTokenHelper.GetVerifierToken(requestToken, TestCreds.Dan.Consumer,
                                                                             TestCreds.Dan.User);

            // Assert
            Assert.Fail("Expected ServerException not thrown.");
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