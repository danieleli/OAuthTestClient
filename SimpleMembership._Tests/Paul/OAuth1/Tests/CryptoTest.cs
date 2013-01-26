using System;
using System.Collections.Generic;
using NUnit.Framework;
using SimpleMembership._Tests.Paul.OAuth1.Crypto;
using log4net;

namespace SimpleMembership._Tests.Paul.OAuth1.Tests
{
    [TestFixture]
    public class CryptoTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(CryptoTest));

        [Test]
        public void StringifyParams_Should_EscapeValues()
        {
            // Arrange
            var key = "somekey";
            var value = "valuewithequal=";
            var d = new SortedDictionary<string, string>();
            d.Add("somekey", "valuewithequal=");

            // Act
            var result = AuthorizationHeaderFactory.Stringify(d);
            LOG.Debug("Result: " + result);

            // Assert
            Assert.IsNotNullOrEmpty(result, "Result");
            var expected = key + "=\"" + Uri.EscapeDataString(value) + "\"";
            Assert.That(result, Is.EqualTo(expected), "Result");
        }

        [Test]
        public void StringifyParams_Should_SortParamsByKey()
        {
            var d = new SortedDictionary<string, string>();
            d.Add("a", "a");
            d.Add("c", "b");
            d.Add("b", "c");


            // Act
            var result = AuthorizationHeaderFactory.Stringify(d);
            LOG.Debug("Result: " + result);

            // Assert
            Assert.IsNotNullOrEmpty(result, "Result");
            var expected = "a=\"a\"," + "b=\"c\"," + "c=\"b\"";
            Assert.That(result, Is.EqualTo(expected), "Result");
        }
    }
}