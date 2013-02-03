using System.Net;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth.Factories;
using SS.OAuth.Helpers;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;
using log4net;

namespace SS.OAuth.Tests.Requests
{
    [TestFixture]
    public class RequestTokenEndpointTest
    {
        private static readonly ILog LOG                                  = LogManager.GetLogger(typeof(RequestTokenEndpointTest));
        private readonly Creds _user                                      = G.DanUser;
        private readonly Creds _consumer                                  = G.DanTestAppConsumer;
        readonly HttpClient _httpClient                                   = new HttpClient();

        [Test]
        public void HappyPath()
        {
            // Arrange            
            var requestParam = new RequestTokenParams(_consumer);
            var msgFactory = new RequestTokenMessageFactory(requestParam);
            var msg = msgFactory.CreateMessage();


            // Act
            var result = _httpClient.SendAsync(msg).Result;
            LOG.Debug("Status: " + result.StatusCode);


            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.StatusCode, Is.EqualTo(HttpStatusCode.OK), "Status Code");
        }

        [Test]
        public void BadConsumerKey_Returns_Unauthorized()
        {
            // Arrange
            var consumer = new Creds("xxxx", _consumer.Secret);
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



        //[Test]
        //public void CallbackPresent_Redirects_ToCallback()
        //{
        //    // Arrange            
        //    var requestInput = new RequestTokenParams(_consumer, "http://www.google.com");

        //    // Act
        //    var requestToken = _cmd.GetToken(requestInput);

        //    // Assert
        //    Assert.Ignore("Not Implemented");
        //}

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

        //[Test, ExpectedException(ExpectedException = typeof(UnauthorizedAccessException))]
        //public void BadConsumerKey_Throws_UnauthorizedException()
        //{
        //    // Arrange
        //    var consumer = new Creds("xxxx", _consumer.Secret);
        //    var input = new RequestTokenParams(consumer);

        //    // Act
        //    var requestToken = _cmd.GetToken(input);

        //    // Asset
        //    Assert.Fail("Exception not thrown.");
        //}


        //[Test, ExpectedException(ExpectedException = typeof(UnauthorizedAccessException))]
        //public void BadConsumerSecret_Throws_UnauthorizedException()
        //{
        //    // Arrange
        //    var consumer = new Creds(_consumer.Key, "bad_secret");
        //    var input = new RequestTokenParams(consumer);

        //    // Act
        //    var requestToken = _cmd.GetToken(input);

        //    // Assert
        //    Assert.Fail("Exception not thrown.");
        //}


        //[Test,
        //    ExpectedException(ExpectedException = typeof(UnauthorizedAccessException),
        //        ExpectedMessage = G.INVALID_SIGNATURE_MESSAGE)]
        //public void BadSignature_Throws_UnauthorizedAccessException_With_InvalidSignatureContent()
        //{
        //    // Arrange
        //    var consumer = new Creds(_consumer.Key, "dsds");
        //    var input = new RequestTokenParams(consumer);

        //    // Act
        //    var requestToken = _cmd.GetToken(input);

        //    // Assert
        //    Assert.Fail("No Exception Thrown");
        //}

        //[Test]
        //public void GetRequestToken_Success()
        //{
        //    // Arrange
        //    var input = new RequestTokenParams(_consumer);

        //    // Act
        //    var requestToken = _cmd.GetToken(input);

        //    // Assert
        //    Assert.IsNotNull(requestToken, "RequestToken");
        //    Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
        //    Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Secret");
        //}

        //[Test]
        //public void GetRequestToken_WithPlusInUserName()
        //{
        //    // Arrange
        //    var input = new RequestTokenParams(_user);

        //    // Act
        //    var requestToken = _cmd.GetToken(input);

        //    // Assert
        //    Assert.IsNotNull(requestToken, "RequestToken");
        //    Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
        //    Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Secret");
        //}

        //[Test]
        //public void MissingAuthHeader_Returns_UnauthorizedStatusCode()
        //{
        //    //// Arrange
        //    //var parameters = new RequestTokenParams(_consumer);
        //    //var p = new Commands.GetTokenCommand();
        //    //var msg = p.CreateMessage(parameters);
        //    //var sender = new MessageSender();

        //    //// Act
        //    //var response = sender.Send(msg);

        //    //// Assert
        //    //Assert.That(response.StatusCode, Is.EqualTo(HttpStatusCode.Unauthorized));
        //}

        //[Test]
        //public void CreateHeader_Contains_ConsumerKey()
        //{
        //    //    // Arrange
        //    //    const string key = "keyABC";
        //    //    const string secret = "secretDEF";
        //    //    var consumer = new Creds(key, secret);
        //    //    var param = new RequestTokenParams(consumer);

        //    //    // Act
        //    //    var header = param.GetOAuthHeader();

        //    //    // Assert
        //    //    LOG.Info(header);
        //    //    Assert.IsNotNullOrEmpty(header, "header");
        //    //    Assert.That(header, Contains.Substring(key), "consumer key");
        //    //    Assert.That(header, Is.Not.ContainsSubstring(secret), "consumer secret");
    }
}
