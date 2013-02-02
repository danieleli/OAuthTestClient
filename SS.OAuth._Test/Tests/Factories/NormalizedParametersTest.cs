using System;
using System.Collections.Specialized;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth.Extensions;
using SS.OAuth.Factories;
using SS.OAuth.Helpers;
using SS.OAuth.Models;
using log4net;

namespace SS.OAuth.Tests.Factories
{

    [TestFixture]
    public class NormalizedParametersTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(NormalizedParametersTest));

        const string URL = "HTTPS://www.ExAMplwwwe.com/Auth?a=valuea&z=valuez&b=valueb1&b=valueb2";
        readonly HttpMethod _method = HttpMethod.Get;
        readonly TestParams _testParam;
        Uri _uri;

        public NormalizedParametersTest()
        {
            // Arrange
            const string cKey = "consumerKey";
            const string cSecret = "consumerSecret";
            var consumer = new Creds(cKey, cSecret);


            _testParam = new TestParams(consumer, "123", "456");
            _uri = new Uri(URL);

        }

        [Test]
        public void Include_QueryStringParams()
        {
            // Arrange 
            var sigFactory = new SignatureFactory(_testParam, _method, _uri);
            
            // Act
            var normalizedRequestParams = sigFactory.GetAllRequestParameters();

            // Assert
            Assert.That(normalizedRequestParams, Is.Not.Null, "normalizedRequestParams");

            var valuesA = normalizedRequestParams.GetValues("a");
            Assert.That(valuesA, Is.Not.Null, "valuesUnderKeyA");
            Assert.That(valuesA.Length, Is.EqualTo(1), "Count of RequestParams Named 'a'");

            var valuesB = normalizedRequestParams.GetValues("b");
            Assert.That(valuesB, Is.Not.Null, "valuesUnderKeyB");
            Assert.That(valuesB.Length, Is.EqualTo(2), "Count of RequestParams Named 'b'");
        }

        [Test]
        public void Includes_ItemWithName_ConsumerKey()
        {
            // Arrange 
            var sigFactory = new SignatureFactory(_testParam, _method, _uri);

            // Act
            var normalizedRequestParams = sigFactory.GetAllRequestParameters();

            // Assert
            var values = normalizedRequestParams.GetValues(OAuth.V1.Keys.CONSUMER_KEY);
            Assert.That(values, Is.Not.Null, "oauthConsumerTokenKey_Values");
            Assert.That(values.Length, Is.EqualTo(1), "Count of RequestParams Named " + OAuth.V1.Keys.CONSUMER_KEY);
            LOG.Debug("normal params.Stringify: " + normalizedRequestParams.Stringify());
        }

        [Test]
        public void Includes_EntityBody()
        {
            // Arrange
            var key = "name1";
            var value = "value1";
            var content = new NameValueCollection { { key, value } };
            var sigFactory = new SignatureFactory(_testParam, _method, _uri);

            // Act
            var normalizedRequestParams = sigFactory.GetAllRequestParameters();

            // Assert;
            var values = normalizedRequestParams.GetValues(key);
            Assert.That(values, Is.Not.Null, key);
            Assert.That(values.Length, Is.EqualTo(1), "Count of " + key);
            Assert.That(values[0], Is.EqualTo(value), "Value: " + value);
            LOG.Debug("normal params.Stringify: " + normalizedRequestParams.Stringify());
        }

    }
}
