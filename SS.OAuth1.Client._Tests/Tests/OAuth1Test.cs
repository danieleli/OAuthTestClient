#region

using NUnit.Framework;
using SS.OAuth1.Client.Composers;
using SS.OAuth1.Client.Parameters;
using log4net;

#endregion

namespace SS.OAuth1.Client._Tests.Tests
{
    [TestFixture]
    public class OAuth1Test
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (OAuth1Test));
        private const string PASSWORD_SHA1 = "5ravvW12u10gQVQtfS4/rFuwVZM="; // password1234
        private readonly Creds _user;

        public OAuth1Test()
        {
            _user = new Creds("dantest", PASSWORD_SHA1);
        }

        [Test] 
        public void TwoLegged_Success()
        {
            // Arrange            
            var requestInput = new RequestTokenParameters(_user);
            var requestToken = RequestTokenComposer.GetRequstToken(requestInput);
            var input = new AccessTokenParameters(_user, requestToken);

            // Act
            var accessToken = AccessTokenComposer.GetAccessToken(input);

            // Assert
            Assert.IsNotNull(accessToken, "AccessToken");
            Assert.IsNotNullOrEmpty(accessToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(accessToken.Secret, "oauth_token_secret");
        }
    }
}