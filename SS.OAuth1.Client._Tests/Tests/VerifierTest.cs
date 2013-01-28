﻿#region

using System;
using System.Net;
using System.Text;
using NUnit.Framework;
using log4net;

#endregion

namespace SS.OAuth1.Client._Tests.Tests
{
    [TestFixture]
    public class VerifierTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (VerifierTest));
        private readonly Creds _user = G.TestCreds.DanUser;
        private readonly Creds _consumer = G.TestCreds.DanApp;

        public static string EncodeTo64(string toEncode)
        {
            var toEncodeAsBytes
                = Encoding.UTF8.GetBytes(toEncode);
            var returnValue
                = Convert.ToBase64String(toEncodeAsBytes);
            return returnValue;
        }

        [Test]
        public void AuthorizeWithInvalidToken_Returns_BadRequest()
        {
            //// Arrange
            //var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(_consumer, "oob");
            //requestToken.Key = "xxxx";

            //// Act
            //var response = RequestComposer.VerifierTokenComposer.GetAuthorizeResponse(requestToken);

            //// Assert
            //LOG.Debug("Response: " + response);
            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.BadRequest), "HttpStatusCode");
        }

        [Test]
        public void Authorize_Returns_StatusCodeOk()
        {
            //// Arrange
            //var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(_consumer, "oob");

            //// Act
            //var response = RequestComposer.VerifierTokenComposer.GetAuthorizeResponse(requestToken);

            //// Assert
            //LOG.Debug("Response: " + response);
            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "HttpStatusCode");
        }

        [Test, ExpectedException(ExpectedException = typeof (UnauthorizedAccessException))]
        public void BadVerifier_Returns_UnauthorizedStatusCode()
        {
            //// Arrrange
            //var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(_consumer, "oob");

            //// Act

            //// Assert
            //Assert.Ignore("Not Implemented");
        }

        [Test]
        public void GetAccessToken_By_SimulatedUserLogin()
        {
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
            //// Arrange
            //var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(_consumer, "oob");

            //// Act
            //var verifier = RequestComposer.VerifierTokenComposer
            //                            .GetVerifierToken(requestToken, _consumer, _user);

            //// Assert
            //LOG.Debug("Verifier: " + verifier);
            //Assert.IsNotNullOrEmpty(verifier.Secret, "Verifier");
        }

        [Test]
        public void When_ActiveMxMerchantUserSession_AuthorizeToken_Displays_ApproveDenyView()
        {
            //Assert.Ignore("Not Implemented");
        }

        [Test]
        public void When_NoActiveUserSessionOnMxMerchant_AuthorizeToken_RedirectsToLogin()
        {
            //// Arrange
            //var requestToken = RequestComposer.RequestTokenComposer.GetRequstToken(_consumer, "oob");

            //// Act
            //var response = RequestComposer.VerifierTokenComposer
            //                            .GetAuthorizeResponse(requestToken);


            //// Assert
            //LOG.Debug("Response: " + response);
            //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Redirect), "Response.StatusCode");
            //Assert.That(response.Headers.Location, Contains.Substring("login"));
        }
    }
}