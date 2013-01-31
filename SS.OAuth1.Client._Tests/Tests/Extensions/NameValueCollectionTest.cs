#region

using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;
using SS.OAuth1.Client.Extensions;
using log4net;

#endregion

namespace SS.OAuth1.Client._Tests.Tests.Extensions
{
    [TestFixture]
    public class NameValueCollectionTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (NameValueCollectionTest));

        [Test]
        public void Stringify_Escapes_KeysAndValues()
        {
            // Arrange
            var key = "some+key";
            var value = "valuewith=equal";
            var d = new NameValueCollection();
            d.Add(key, value);

            // Act
            var result = d.Stringify();
            LOG.Debug("Result: " + result);

            // Assert
            Assert.IsNotNullOrEmpty(result, "Result");
            var expected = key.UrlEncodeForOAuth() + "=\"" + value.UrlEncodeForOAuth() + "\"";
            Assert.That(result, Is.EqualTo(expected), "Result");
        }

        [Test]
        public void Stringify_Sorts_ByKey()
        {
            var d = new NameValueCollection();
            d.Add("a", "a");
            d.Add("c", "b");
            d.Add("b", "c");


            // Act
            var result = d.Stringify();
            LOG.Debug("Result: " + result);

            // Assert
            Assert.IsNotNullOrEmpty(result, "Result");
            var expected = "a=\"a\"," + "b=\"c\"," + "c=\"b\"";
            Assert.That(result, Is.EqualTo(expected), "Result");
        }

        [Test]
        public void Stringify_Sorts_ByKey_ThenValue()
        {
            var d = new NameValueCollection();
            d.Add("c", "z");
            d.Add("a", "c");
            d.Add("a", "a");
            d.Add("a", "b");
            d.Add("d", "y");

            // Act
            var result = d.Stringify();
            LOG.Debug("Result: " + result);

            // Assert
            Assert.IsNotNullOrEmpty(result, "Result");
            
            // expected string =   a="a",a="b",a="c",c="z",d="y"
            const string expected = "a=\"a\"," + "a=\"b\"," + "a=\"c\"," + "c=\"z\"," + "d=\"y\"";
            Assert.That(result, Is.EqualTo(expected), "Result");
        }
    }
}