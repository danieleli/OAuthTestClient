using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Web.WebPages.OAuth;
using NUnit.Framework;
using Rhino.Mocks;
using SimpleMembership.Auth.OAuth1a;

namespace SimpleMembership._Tests.Controllers
{
    [TestFixture]
    public class MxClient_Tests
    {
        [Test]
        public void Test1()
        {
            TestAuthConfig.RegisterAuth();
            OAuthWebSecurity.Login("MxMerchant", providerUserId: "", createPersistentCookie: false);

        }

        [Test]
        public void TokenManager()
        {
            var mxClient = new MxClient("a","b");
            Assert.NotNull(mxClient, "mxClient");
            var tokens = mxClient.TokenManager;
            Assert.NotNull(tokens, "tokens");

        }
    }
}
