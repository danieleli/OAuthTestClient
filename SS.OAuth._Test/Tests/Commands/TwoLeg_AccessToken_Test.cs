using System;
using System.Net.Http;
using NUnit.Framework;
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

        [Test, ExpectedException(typeof(UnauthorizedAccessException))]
        public void RequestTokenNotFromCurrentUser__Returns_401WithForeignConsumerCredentialsMessage()
        {
            // Arrange            
            var foreignConsumer = G.DanTestAppConsumer;
            var user = G.DanUser;
            var requestTokenInput = new RequestTokenParams(foreignConsumer);

            var cmd = new object(); ;//GetTokenCommand();
            var requestToken = new Creds("","");

            var input = new AccessTokenParams(user, requestToken, null);

            // Act
            try
            {
                var accessToken = new object(); //cmd.GetToken(input);
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

        [Test]
        public void TwoLegged()
        {
            var user = G.DanUser;
            var requestParams = new RequestTokenParams(user);
            
            var requestCmd = new GetRequestTokenCommand();
            var requestToken = requestCmd.GetToken(requestParams);

            var accessTokenParams = new AccessTokenParams(user, requestToken, null);

            var accessCmd = new GetAccessTokenCommand();
            var accessToken = accessCmd.GetToken(accessTokenParams);

            Assert.That(accessToken, Is.Not.Null, "AccessToken");
            Assert.That(accessToken.Key, Is.Not.Null, "AccessToken.Key");
            Assert.That(accessToken.Key, Is.Not.Empty, "AccessToken.Key");
            Assert.That(accessToken.Secret, Is.Not.Null, "AccessToken.Secret");
            Assert.That(accessToken.Secret, Is.Not.Empty, "AccessToken.Secret");
        }


        //[Test]
        //public void TwoLegged_Success()
        //{
        //    // Arrange            
        //    var cmd = new Commands.GetTokenCommand();
        //    var requestInput = new RequestTokenParameters(_user);
        //    var requestToken = cmd.GetToken(requestInput);
        //    LOG.LogCreds("requestToken", requestToken);

        //    // Act
        //    var input = new AccessTokenParameters(_user, requestToken, null);
        //    var accessToken = cmd.GetToken(input);

        //    // Assert
        //    Assert.IsNotNull(accessToken, "AccessToken");
        //    Assert.IsNotNullOrEmpty(accessToken.Key, "oauth_token");
        //    Assert.IsNotNullOrEmpty(accessToken.Secret, "oauth_token_secret");
        //    LOG.LogCreds("accessToken", accessToken);
        //}
    }


    public class GetAccessTokenCommand
    {
        public Creds GetToken(AccessTokenParams p)
        {
            throw new NotImplementedException();
        }
    }

    public class GetRequestTokenCommand
    {
        public Creds GetToken(RequestTokenParams p)
        {
            throw new NotImplementedException();
        }
    }
}