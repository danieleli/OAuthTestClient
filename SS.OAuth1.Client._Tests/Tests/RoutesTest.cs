#region

using NUnit.Framework;
using SS.OAuth1.Client.Helpers;

#endregion

namespace SS.OAuth1.Client._Tests.Tests
{
    [TestFixture]
    public class RoutesTest
    {
        [Test]
        public void AuthorizeTokenWebViewRoute_Is_Http()
        {
            var url = OAuth.V1.Routes.WebViews.GetAuthorizeRoute("xxx");
            Assert.That(url.ToLower().Contains("https"), Is.EqualTo(false), "https found");
            Assert.That(url.ToLower().Contains("http"), Is.EqualTo(true), "http found");
        }
    }
}