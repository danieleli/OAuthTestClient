using System;
using System.Collections.Specialized;
using NUnit.Framework;
using SS.OAuth.Helpers;
using SS.OAuth.Models;
using log4net;

namespace SS.OAuth1.Client._Tests.Tests.Helpers
{

    [TestFixture]
    public class NormalizedParametersTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(NormalizedParametersTest));
        private readonly TestParameter _testParam;

        public NormalizedParametersTest()
        {
            // Arrange
            const string cKey = "consumerKey";
            const string cSecret = "consumerSecret";
            var consumer = new Creds(cKey, cSecret);
            

            _testParam = new TestParameter(consumer);
            
        }

        [Test]
        public void Include_QueryStringParams()
        {
            // Act
            var normalizedRequestParams = _testParam.GetAllRequestParameters(null);

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
            // Act
            var normalizedRequestParams = _testParam.GetAllRequestParameters(null);

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
            var content = new NameValueCollection {{key, value}};
            
            // Act
            var normalizedRequestParams = _testParam.GetAllRequestParameters(content);

            // Assert;
            var values = normalizedRequestParams.GetValues(key);
            Assert.That(values, Is.Not.Null, key);
            Assert.That(values.Length, Is.EqualTo(1), "Count of " + key);
            Assert.That(values[0], Is.EqualTo(value), "Value: " + value);
            LOG.Debug("normal params.Stringify: " + normalizedRequestParams.Stringify());
        }

    }
}
