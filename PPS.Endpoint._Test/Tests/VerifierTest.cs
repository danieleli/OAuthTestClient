#region

using System;
using System.Collections.Specialized;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using NUnit.Framework;
using PPS.Endpoint.Helpers;
using SS.OAuth;
using SS.OAuth.Commands;
using SS.OAuth.Extensions;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;
using log4net;

#endregion

namespace PPS.Endpoint.Tests
{
    [TestFixture,Ignore]
    public class VerifierTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (VerifierTest));

        private readonly Creds _consumer = G.DanTestAppConsumer;
        private readonly Creds _user = G.DanUser;

        private static class Helper
        {
            public static void HitWebView(string token)
            {
                var webClient = new HttpClient();
                var url = OAuth.V1.Routes.WebViews.GetAuthorizeRoute(token);
                var uri = new Uri(url);
                var msg = new HttpRequestMessage(HttpMethod.Get, uri);
                var response = webClient.SendAsync(msg).Result;
                LOG.Debug("Response: " + response);
            }
        }

        
        [Test]
        public void HappyPath()
        {
            var consumer = G.DanTestAppConsumer;
            var rToken = TokenHelper.GetRequestToken(consumer);
            var verifierParams = new VerifierTokenParams(consumer, rToken, rToken.Key);
            var verifierCommand = new GetVerifierTokenCommand(verifierParams);
        }


        [Test]
        public void AuthorizeWithInvalidToken_Returns_BadRequest()
        {
            throw new NotImplementedException();
            //// Arrange
            //var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(_consumer, "oob");
            //requestToken.Key = "xxxx";

            //// Act
            //var response = RequestComposer.GetVerifierCommand.GetAuthorizeResponse(requestToken);

            //// Assert
            //LOG.Debug("Response: " + response);
            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "HttpStatusCode");
        }

        [Test]
        public void Authorize_Returns_StatusCodeOk()
        {
            throw new NotImplementedException();
            //// Arrange
            //var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(_consumer, "oob");

            //// Act
            //var response = RequestComposer.GetVerifierCommand.GetAuthorizeResponse(requestToken);

            //// Assert
            //LOG.Debug("Response: " + response);
            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HttpStatusCode");
        }

        [Test, ExpectedException(ExpectedException = typeof (UnauthorizedAccessException))]
        public void BadVerifier_Returns_UnauthorizedStatusCode()
        {
            throw new NotImplementedException();
            //// Arrrange
            //var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(_consumer, "oob");

            //// Act

            //// Assert
            //Assert.Ignore("Not Implemented");
        }

        [Test]
        public void GetAccessToken_By_SimulatedUserLogin()
        {
            throw new NotImplementedException();
            //    var passwordSha1 = "5ravvW12u10gQVQtfS4/rFuwVZM="; // password1234
            //    var user = new Creds("dantest", passwordSha1);

            //    var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(user, "oob");

            //    // Use RequestToken as verifier after encoding secret.
            //    var verifier = new Creds(requestToken.Key, EncodeTo64(requestToken.Secret));

            //    var accessToken = RequestComposer.AccessTokenComposer.GetAccessToken(user, verifier);

            //    Assert.IsNotNull(accessToken, "AccessToken");
        }

        [Test]
        public void GetVerifier()
        {
            throw new NotImplementedException();
            //// Arrange
            //var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(_consumer, "oob");

            //// Act
            //var verifier = RequestComposer.GetVerifierCommand
            //                            .GetVerifierToken(requestToken, _consumer, _user);

            //// Assert
            //LOG.Debug("Verifier: " + verifier);
            //Assert.IsNotNullOrEmpty(verifier.Secret, "Verifier");
        }

        [Test]
        public void Success()
        {
            // Arrange            
            var requestToken = TokenHelper.GetRequestToken(_consumer);
            Helper.HitWebView(requestToken.Key);
            var twoLegAccessToken = TokenHelper.GetTwoLegAccessToken(_user);


            var verifierParams = new VerifierTokenParams(_user, twoLegAccessToken, requestToken.Key);
            var verifierCmd = new GetVerifierTokenCommand(verifierParams);


            // Act
            var verifierToken = verifierCmd.GetToken();


            // Assert
            Assert.IsNotNull(verifierToken, "AccessToken");
            Assert.IsNotNullOrEmpty(verifierToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(verifierToken.Secret, "oauth_token_secret");
            LOG.LogCreds("verifierToken", verifierToken);
        }

        [Test]
        public void When_ActiveMxMerchantUserSession_AuthorizeToken_Displays_ApproveDenyView()
        {
            throw new NotImplementedException();
            //Assert.Ignore("Not Implemented");
        }

        [Test]
        public void When_NoActiveUserSessionOnMxMerchant_AuthorizeToken_RedirectsToLogin()
        {
            throw new NotImplementedException();
            //// Arrange
            //var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(_consumer, "oob");

            //// Act
            //var response = RequestComposer.GetVerifierCommand
            //                            .GetAuthorizeResponse(requestToken);


            //// Assert
            //LOG.Debug("Response: " + response);
            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect), "Response.StatusCode");
            //Assert.That(response.Headers.Location, Contains.Substring("login"));
        }
    }
}