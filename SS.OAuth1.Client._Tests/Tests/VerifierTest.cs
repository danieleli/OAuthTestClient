#region

using System;
using System.Net;
using System.Net.Http;
using System.Text;
using NUnit.Framework;
using SS.OAuth1.Client.Commands;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;
using log4net;

#endregion

namespace SS.OAuth1.Client._Tests.Tests
{
    [TestFixture]
    public class VerifierTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(VerifierTest));

        private readonly Creds _consumer = G.TestCreds.DanConsumer;
        private readonly Creds _user = G.TestCreds.DanUser;


        public static string EncodeTo64(string toEncode)
        {
            var toEncodeAsBytes
                = Encoding.UTF8.GetBytes(toEncode);
            var returnValue
                = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        [Test]
        public void Success()
        {
            // Arrange            
            var requestToken = GetRequestToken(_consumer);
            HitWebView(requestToken.Key);
            var twoLegAccessToken = GetTwoLegAccessToken(_user);
            var fuckedToken = new Creds(twoLegAccessToken.Key, requestToken.Secret);

            

            // todo: use two leg to get access token like the user signed on to mxmerchant site
            var verifierParams = new VerifierTokenParameters(_consumer, fuckedToken);
            var verifierCmd = new GetVerifierCommand();

            
            // Act
            var verifierToken = verifierCmd.GetToken(verifierParams);


            // Assert
            Assert.IsNotNull(verifierToken, "AccessToken");
            Assert.IsNotNullOrEmpty(verifierToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(verifierToken.Secret, "oauth_token_secret");
            LOG.LogCreds("verifierToken", verifierToken);
        }


        private static void HitWebView(string token)
        {
            var webClient = new HttpClient();
            var url = OAuth.V1.Routes.WebViews.GetAuthorizeRoute(token);
            var uri = new Uri(url);
            var msg = new HttpRequestMessage(HttpMethod.Get, uri);
            var response = webClient.SendAsync(msg).Result;
            LOG.Debug("Response: " + response);
        }

        private static Creds GetTwoLegAccessToken(Creds user)
        {
            var requestToken = GetRequestToken(user);
            var input = new AccessTokenParameters(user, requestToken, null);
            var cmd = new GetTokenCommand();
            var accessToken = cmd.GetToken(input);

            return accessToken;
        }

        private static Creds GetRequestToken(Creds consumer)
        {
            var requestTokenCmd = new GetTokenCommand();
            LOG.LogCreds("consumer", consumer);
            var requestInput = new RequestTokenParameters(consumer);
            var requestToken = requestTokenCmd.GetToken(requestInput);
            LOG.LogCreds("requestToken", requestToken);
            return requestToken;
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