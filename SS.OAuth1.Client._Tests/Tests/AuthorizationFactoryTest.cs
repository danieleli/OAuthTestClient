#region

using System;
using System.Collections.Generic;
using NUnit.Framework;
using log4net;

#endregion

namespace SS.OAuth1.Client._Tests.Tests
{
    [TestFixture]
    public class AuthorizationFactoryTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (AuthorizationFactoryTest));

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