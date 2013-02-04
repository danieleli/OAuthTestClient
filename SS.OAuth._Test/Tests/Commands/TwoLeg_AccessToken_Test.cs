using System;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth.Commands;
using SS.OAuth.Extensions;
using SS.OAuth.Factories;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;
using log4net;

namespace SS.OAuth.Tests.Commands
{
    [TestFixture]
    public class TwoLeg_AccessToken_Test
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(TwoLeg_AccessToken_Test));

        /// <summary>
        /// Two leg oauth should use SAME user and consumer.  Different user and consumer is three leg
        /// and requires a verifier token.
        /// </summary>
        [Test, ExpectedException(typeof(UnauthorizedAccessException))]
        public void RequestTokenNotFromCurrentUser__Returns_401WithForeignConsumerCredentialsMessage()
        {
            // Arrange            
            var foreignConsumer     = G.DanTestAppConsumer;
            var foreignRequestToken = GetRequestToken(foreignConsumer);

            var user                = G.DanUser;
            var accessTokenParams   = new AccessTokenParams(user, foreignRequestToken, null);
            var accessTokenCommand  = new GetAccessTokenCommand(accessTokenParams);

            // Act
            try
            {
                var accessToken = accessTokenCommand.GetToken();
            }
            catch (UnauthorizedAccessException uae)
            {
                var msg = uae.Message;
                if (msg.ToLower().Contains("foreign consumer credentials"))
                {
                    throw;
                }

                Assert.Fail(
                    "Expected UnauthorizedAccessException found but 'request token acquired by foreign consumer credentials' msg not found.");
            }
        }

        private static Creds GetRequestToken(Creds consumer)
        {
            var requestTokenParams = new RequestTokenParams(consumer);
            var cmd                = new GetRequestTokenCommand(requestTokenParams);
            var requestToken       = cmd.GetToken();
            
            return requestToken;
        }

        [Test]
        public void TwoLegged()
        {
            var user              = G.DanUser;
            var requestToken      = GetRequestToken(user);
            var accessTokenParams = new AccessTokenParams(user, requestToken, null);
            var accessCmd         = new GetAccessTokenCommand(accessTokenParams);
            var accessToken       = accessCmd.GetToken();

            LOG.LogCreds("AccessToken", accessToken);
            Assert.That(accessToken, Is.Not.Null, "AccessToken");
            Assert.That(accessToken.Key, Is.Not.Null, "AccessToken.Key");
            Assert.That(accessToken.Key, Is.Not.Empty, "AccessToken.Key");
            Assert.That(accessToken.Secret, Is.Not.Null, "AccessToken.Secret");
            Assert.That(accessToken.Secret, Is.Not.Empty, "AccessToken.Secret");

        }
    }
}