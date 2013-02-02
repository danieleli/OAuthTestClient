#region

using System;
using NUnit.Framework;
using SS.OAuth1.Client.Extensions;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;
using log4net;

#endregion

namespace SS.OAuth1.Client._Tests.Tests.GetTokenCommand
{
    [TestFixture]
    public class TwoLeg_AccessToken_Test
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(TwoLeg_AccessToken_Test));

        private readonly Creds _consumer = G.TestCreds.DanConsumer;
        private readonly Creds _user = G.TestCreds.DanUser;


        [Test] 
        public void TwoLegged_Success()
        {
            // Arrange            
            var cmd = new Commands.GetTokenCommand();
            var requestInput = new RequestTokenParameters(_user);
            var requestToken = cmd.GetToken(requestInput);
            LOG.LogCreds("requestToken", requestToken);

            // Act
            var input = new AccessTokenParameters(_user, requestToken, null);
            var accessToken = cmd.GetToken(input);

            // Assert
            Assert.IsNotNull(accessToken, "AccessToken");
            Assert.IsNotNullOrEmpty(accessToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(accessToken.Secret, "oauth_token_secret");
            LOG.LogCreds("accessToken", accessToken);
        }



        [Test, ExpectedException(typeof(UnauthorizedAccessException))]
        public void RequestTokenNotFromCurrentUser__Returns_401WithForeignConsumerCredentialsMessage()
        {
            // Arrange            
            var cmd = new Commands.GetTokenCommand();
            var requestInput = new RequestTokenParameters(_consumer);
            var requestToken = cmd.GetToken(requestInput);
            var input = new AccessTokenParameters(_user, requestToken, null);

            // Act
            try
            {
                var accessToken = cmd.GetToken(input);
            }
            catch (UnauthorizedAccessException uae)
            {
                var msg = uae.Message;
                if (msg.ToLower().Contains("foreign consumer credentials"))
                {
                    throw;    
                }

                Assert.Fail("Expected UnauthorizedAccessException found but 'request token acquired by foreign consumer credentials' msg not found.");
            }
        }


    }
}