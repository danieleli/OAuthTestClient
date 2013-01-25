#region

using NUnit.Framework;
using SimpleMembership._Tests.Paul;
using log4net;

#endregion

namespace MXM.API.Test.Controllers
{
    [TestFixture]
    public class OAuth2Test
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (OAuth2Test));

        public static Creds GetThreeLegAccessToken(Creds consumer, Creds user, string returnUrl)
        {
            var code = OAuth2Helper.GetAuthorizationCode(consumer, returnUrl).Content.ToString();
            var token = OAuth2Helper.GetAccessToken( code,consumer);

            return null; // accessToken;
        }

        [Test] // Three leg has different consumer and user creds.
        public void ThreeLegged_Success()
        {
            Assert.Ignore("Not Implemented");
            //GetThreeLegAccessToken(consumer, user, returnUrl);
        }

        [Test] // Two leg has same consumer and user creds.
        public static void TwoLegged_Success()
        {
            Assert.Ignore("Not Implemented");
            //GetThreeLegAccessToken(user, user, returnUrl);
        }
    }
}