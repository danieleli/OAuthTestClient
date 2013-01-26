#region

using NUnit.Framework;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1.Tests
{
    [TestFixture]
    public class OAuth1Test
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (OAuth1Test));

        public static class Helper
        {
            public static Creds GetThreeLegAccessToken(Creds consumer, Creds user, string returnUrl)
            {
                var requestToken = RequestHelper.RequestTokenHelper.GetRequstToken(consumer, returnUrl);
                var verifier = RequestHelper.VerifierTokenHelper.GetVerifierToken(requestToken, consumer, user);
                var accessToken = RequestHelper.AccessTokenHelper.GetAccessToken(consumer, verifier);

                return accessToken;
            }

            public static void GetTwoLegAccessToken(Creds user, string returnUrl)
            {
                GetThreeLegAccessToken(user, user, returnUrl);
            }
        }


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
            var accessToken = Helper.GetThreeLegAccessToken(TestCreds.Dan.User, TestCreds.Dan.User, "oob");

            // Assert
            Assert.IsNotNull(accessToken, "AccessToken");
            Assert.IsNotNullOrEmpty(accessToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(accessToken.Secret, "oauth_token_secret");
        }
    }
}