﻿using System.Collections.Generic;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth.Extensions;
using SS.OAuth.Factories;
using SS.OAuth.Helpers;
using SS.OAuth.Models;
using log4net;

namespace SS.OAuth.Tests.Factories.Signature
{

    [TestFixture]
    public class NormalizeParamsTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(NormalizeParamsTest));

        const string URL           = "HTTPS://www.ExAMplwwwe.com/Auth?a=valuea&z=valuez&b=valueb1&b=valueb2";
        private const string KEY   = "name1";
        private const string VALUE = "value1";

        private readonly HttpRequestMessage _httpMessage;
        readonly TestParams _testParam;


        public NormalizeParamsTest()
        {
            // Arrange
            const string cKey    = "consumerKey";
            const string cSecret = "consumerSecret";
            var consumer         = new Creds(cKey, cSecret);

            _testParam   = new TestParams(consumer, "123", "456");
            _httpMessage = new HttpRequestMessage(HttpMethod.Get, URL);

            var contentDictionary = new Dictionary<string, string> {{KEY, VALUE}};
            _httpMessage.Content  = new FormUrlEncodedContent(contentDictionary);
        }


        [Test]
        public void Include_QueryStringParams()
        {
            // Arrange 
            var sigFactory = new OAuthHeaderFactory(_testParam);

            // Act
            var normalizedRequestParams = sigFactory.SignatureBaseStringFactory.GetAllRequestParameters(_httpMessage);

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
            var sigFactory = new OAuthHeaderFactory(_testParam);

            // Act
            var normalizedRequestParams = sigFactory.SignatureBaseStringFactory.GetAllRequestParameters(_httpMessage);

            // Assert
            var values = normalizedRequestParams.GetValues(OAuth.V1.Keys.CONSUMER_KEY);
            Assert.That(values, Is.Not.Null, "oauthConsumerTokenKey_Values");
            Assert.That(values.Length, Is.EqualTo(1), "Count of RequestParams Named " + OAuth.V1.Keys.CONSUMER_KEY);
            LOG.Debug("normal params.Stringify: " + normalizedRequestParams.Stringify());
        }


        [Test]
        public void Includes_EntityBody()
        {
            var sigFactory = new OAuthHeaderFactory(_testParam);

            // Act
            var normalizedRequestParams = sigFactory.SignatureBaseStringFactory.GetAllRequestParameters(_httpMessage);

            // Assert;
            var values = normalizedRequestParams.GetValues(KEY);
            Assert.That(values, Is.Not.Null, KEY);
            Assert.That(values.Length, Is.EqualTo(1), "Count of " + KEY);
            Assert.That(values[0], Is.EqualTo(VALUE), "Value: " + VALUE);
            LOG.Debug("normal params.Stringify: " + normalizedRequestParams.Stringify());
        }

    }
}
