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



        [Test] // Two leg is same as three leg but reuse consumer creds as user creds.
        public void TwoLegged_Success()
        {
            // Act            
            var requestInput = new RequestTokenParameters(TestCreds.Dan.Consumer);
            var requestToken = RequestComposer.RequestTokenHelper.GetRequstToken(requestInput);

            var accessToken = RequestComposer.AccessTokenHelper.GetAccessToken(TestCreds.Dan.Consumer, requestToken);


            // Assert
            Assert.IsNotNull(accessToken, "AccessToken");
            Assert.IsNotNullOrEmpty(accessToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(accessToken.Secret, "oauth_token_secret");
        }
    }
}