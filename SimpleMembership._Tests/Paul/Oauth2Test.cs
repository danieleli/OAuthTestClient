#region

using NUnit.Framework;
using SimpleMembership._Tests.Paul;
using log4net;

#endregion

namespace MXM.API.Test.Controllers
{
    [TestFixture]
    public class OAuth2Test
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (OAuth2Test));

        public static Creds GetThreeLegAccessToken(Creds consumer, Creds user, string returnUrl)
        {
            var code = OAuth2Helper.GetAuthorizationCode(consumer, returnUrl).Content.ToString();
            var token = OAuth2Helper.GetAccessToken( code,consumer);

            return null; // accessToken;
        }

        public static void GetTwoLegAccessToken(Creds user, string returnUrl)
        {
            GetThreeLegAccessToken(user, user, returnUrl);
        }

        [Test] // Three leg has different consumer and user creds.
        public void ThreeLegged_Success()
        {
            // Act
            var accessToken = GetThreeLegAccessToken(TestCreds.Dan.Consumer, TestCreds.Dan.User, "oob");

            // Assert
            Assert.IsNotNull(accessToken, "AccessToken");
            Assert.IsNotNullOrEmpty(accessToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(accessToken.Secret, "oauth_token_secret");
        }
    }
}