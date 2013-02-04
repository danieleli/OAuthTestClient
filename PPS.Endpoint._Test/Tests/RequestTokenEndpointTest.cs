using System;
using System.Net;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth.Factories;
using SS.OAuth.Helpers;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;
using log4net;

namespace SS.OAuth.Tests.Endpoints
{
    [TestFixture]
    public class RequestTokenEndpointTest
    {
        private static readonly ILog LOG  = LogManager.GetLogger(typeof(RequestTokenEndpointTest));
        private readonly Creds _consumer  = G.DanTestAppConsumer;
        readonly HttpClient _httpClient   = new HttpClient();

        [Test]
        public void HappyPath()
        {
            // Arrange            
            var requestParam = new RequestTokenParams(_consumer);
            var msgFactory   = new RequestTokenMessageFactory(requestParam);
            var msg          = msgFactory.CreateMessage();

            var msg2 = CreateMessage();

            // Act
            var result = _httpClient.SendAsync(msg).Result;
            LOG.Debug("Status: " + result.StatusCode);


            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code");
        }


        private HttpRequestMessage CreateMessage()
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, OAuth.V1.Routes.RequestToken);
            return msg;
        }

        [Test]
        public void NoVersionInHeader_Returns_OK()
        {
            // Arrange            
            var requestParam = new NoVersionRequestTokenParams(_consumer);
            var msgFactory = new RequestTokenMessageFactory(requestParam);
            var msg = msgFactory.CreateMessage();


            // Act
            var result = _httpClient.SendAsync(msg).Result;
            LOG.Debug(result);


            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code");
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

        [Test]
        public void BadConsumerKey_Returns_Unauthorized()
        {
            // Arrange
            var consumer     = new Creds("xxxx", _consumer.Secret);
            var requestParam = new RequestTokenParams(consumer);
            var msgFactory   = new RequestTokenMessageFactory(requestParam);
            var msg          = msgFactory.CreateMessage();


            // Act
            var result = _httpClient.SendAsync(msg).Result;
            LOG.Debug("Status: " + result.StatusCode);


            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Status Code.");
        }


        [Test]
        public void BadConsumerSecret_Returns_Unauthorized()
        {
            // Arrange
            var consumer = new Creds(_consumer.Key,"xxx");
            var requestParam = new RequestTokenParams(consumer);
            var msgFactory = new RequestTokenMessageFactory(requestParam);
            var msg = msgFactory.CreateMessage();


            // Act
            var result = _httpClient.SendAsync(msg).Result;
            LOG.Debug("Status: " + result.StatusCode);


            // Assert
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Status Code.");
        }


        [Test]
        public void BadSignature_Throws_UnauthorizedAccessException_With_InvalidSignatureContent()
        {
            // Arrange
            var consumer     = new Creds(_consumer.Key, "dsds");
            var requestParam = new RequestTokenParams(consumer);
            var msgFactory   = new RequestTokenMessageFactory(requestParam);
            var msg          = msgFactory.CreateMessage();


            // Act
            var response = _httpClient.SendAsync(msg).Result;
            var content  = response.Content.ReadAsStringAsync().Result;
            
            LOG.Debug("Status: " + response.StatusCode);
            LOG.Debug("Content: " + content);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized), "Status Code.");
            Assert.That(content.ToLower(), Contains.Substring("signature"));            
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
