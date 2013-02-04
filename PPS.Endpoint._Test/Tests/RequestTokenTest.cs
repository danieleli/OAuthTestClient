using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using PPS.Endpoint.Helpers;
using SS.OAuth;
using SS.OAuth.Commands;
using SS.OAuth.Factories;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;
using log4net;

namespace PPS.Endpoint.Tests
{
    [TestFixture]
    public class RequestTokenTest
    {
        private static readonly ILog LOG  = LogManager.GetLogger(typeof(RequestTokenTest));
        private readonly Creds _consumer  = G.DanTestAppConsumer;
        readonly HttpClient _httpClient   = new HttpClient();

        [Test]
        public void HappyPath()
        {
            // Arrange            
            var requestParam = new RequestTokenParams(G.DanTestAppConsumer);
            var cmd = new GetRequestTokenCommand(requestParam);

            // Act
            var token = cmd.GetToken();

            // Assert
            AssertTokenOk(token);
        }

        private static void AssertTokenOk(Creds token)
        {
            Assert.That(token, Is.Not.Null);
            Assert.That(token.Key, Is.Not.Null, "Key");
            Assert.That(token.Key, Is.Not.Empty, "Key");
            Assert.That(token.Secret, Is.Not.Null, "Secret");
            Assert.That(token.Secret, Is.Not.Empty, "Secret");
        }

        [Test]
        public void NoVersionInHeader_Returns_Token()
        {
            // Arrange            
            var requestParam = new RequestTokenParams(G.DanTestAppConsumer, null, false);
            var cmd = new GetRequestTokenCommand(requestParam);

            // Act
            var token = cmd.GetToken();

            // Assert
            Assert.That(token, Is.Not.Null);
            Assert.That(token.Key, Is.Not.Null, "Key");
            Assert.That(token.Key, Is.Not.Empty, "Key");
            Assert.That(token.Key, Is.Not.Null, "Secret");
            Assert.That(token.Key, Is.Not.Empty, "Secret");
        }

        [Test]
        public void NoAuthHeader_Returns_UnauthorizedStatus()
        {
            // Arrange            
            var msg = new HttpRequestMessage(HttpMethod.Get, OAuth.V1.Routes.RequestToken);

            // Act
            var result = _httpClient.SendAsync(msg).Result;
            LOG.Debug(result);


            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Status Code");
        }

        [Test]
        public void CallbackPresent_Redirects_ToCallback()
        {
            // Arrange            
            var requestParam = new RequestTokenParams(_consumer, "http://www.example.com/callback");
            var msgFactory = new RequestTokenMessageFactory(requestParam);
            var msg = msgFactory.CreateMessage();


            // Act
            var result = _httpClient.SendAsync(msg).Result;
            LOG.Debug("Status: " + result.StatusCode);


            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Redirect), "Status Code");
        }

        [Test, ExpectedException(typeof(UnauthorizedAccessException))]
        public void BadConsumerKey_Returns_Unauthorized()
        {
            // Arrange
            var consumer     = new Creds("xxxx", _consumer.Secret);
            var requestParam = new RequestTokenParams(consumer);
            var cmd = new GetRequestTokenCommand(requestParam);

            // Act
            var token = cmd.GetToken();
        }


        [Test, ExpectedException(typeof(UnauthorizedAccessException))]
        public void BadConsumerSecret_Returns_Unauthorized()
        {
            // Arrange
            var consumer = new Creds(_consumer.Key,"xxx");
            var requestParam = new RequestTokenParams(consumer);
            var cmd = new GetRequestTokenCommand(requestParam);

            // Act
            var token = cmd.GetToken();
        }


        [Test]
        public void BadSignature_Throws_UnauthorizedAccessException_With_InvalidSignatureContent()
        {
            // Arrange
            var consumer     = new Creds(_consumer.Key, "dsds");
            var requestParam = new RequestTokenParams(consumer);
            var cmd = new GetRequestTokenCommand(requestParam);

            // Act
            try
            {
                var token = cmd.GetToken();
            }
            catch (UnauthorizedAccessException e)
            {

                Assert.That(e.Message.ToLower(), Contains.Substring("invalid"));
                return;
            }

            Assert.Fail("Expected exception (UnauthorizedAccessException) not thrown.");
            
        }

        [Test]
        public void PlusInUsername_Returns_OK()
        {
            // Arrange
            var requestParam = new RequestTokenParams(G.UserWithPlus);
            var msgFactory   = new RequestTokenMessageFactory(requestParam);
            var msg          = msgFactory.CreateMessage();


            // Act
            var response = _httpClient.SendAsync(msg).Result;
            LOG.Debug("Status: " + response.StatusCode);


            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code.");
        }
    }
}
