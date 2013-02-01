#region

using System.Collections.Specialized;
using NUnit.Framework;
using log4net;

#endregion

namespace SS.OAuth.Extensions
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
            var collection = new NameValueCollection();
            collection.Add(key, value);

            // Act
            var result = collection.Stringify();
            LOG.Debug("Result: " + result);

            // Assert
            Assert.IsNotNullOrEmpty(result, "Result");
            var expected = key.UrlEncodeForOAuth() + "=\"" + value.UrlEncodeForOAuth() + "\"";
            Assert.That(result, Is.EqualTo(expected), "Result");
        }

        [Test]
        public void Stringify_Sorts_ByKey()
        {
            var collection = new NameValueCollection();
            collection.Add("a", "a");
            collection.Add("c", "b");
            collection.Add("b", "c");


            // Act
            var result = collection.Stringify();
            LOG.Debug("Result: " + result);

            // Assert
            Assert.IsNotNullOrEmpty(result, "Result");
            var expected = "a=\"a\"," + "b=\"c\"," + "c=\"b\"";
            Assert.That(result, Is.EqualTo(expected), "Result");
        }

        [Test]
        public void Stringify_Sorts_ByKey_ThenValue()
        {
            var collection = new NameValueCollection();
            collection.Add("c", "z");
            collection.Add("a", "c");
            collection.Add("a", "a");
            collection.Add("a", "b");
            collection.Add("d", "y");

            // Act
            var result = collection.Stringify();
            LOG.Debug("Result: " + result);

            // Assert
            Assert.IsNotNullOrEmpty(result, "Result");
            
            // expected string =   a="a",a="b",a="c",c="z",d="y"
            const string expected = "a=\"a\"," + "a=\"b\"," + "a=\"c\"," + "c=\"z\"," + "d=\"y\"";
            Assert.That(result, Is.EqualTo(expected), "Result");
        }
    }
}