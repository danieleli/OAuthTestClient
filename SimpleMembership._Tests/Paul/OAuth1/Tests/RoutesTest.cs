#region

using NUnit.Framework;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1.Tests
{
    [TestFixture]
    public class RoutesTest
    {
        [Test]
        public void AuthorizeTokenRoute_Is_Http()
        {
            var url = OAuth.V1.Routes.GetAuthorizeTokenRoute("xxx");
            Assert.That(url.ToLower().Contains("https"), Is.EqualTo(false), "https found");
            Assert.That(url.ToLower().Contains("http"), Is.EqualTo(true), "http found");
        }
    }
}