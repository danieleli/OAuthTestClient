using System.Collections.Specialized;
using System.Web.Http;
using NUnit.Framework;
using Xunit.Sdk;
using log4net;

namespace MXM.API.Test.Controllers
{
    public static class Keys
    {
        public const string OAUTH_VERIFIER = "oauth_verifier";
        public const string TOKEN = "token";
        public const string TOKEN_SECRET = "token_secret";
    }


    public static partial class OAuthRoutes
    {
        public static class V1A
        {
            public const string ROUTE = G.BASE_URL + "/OAuth/1a/";

            public const string REQUEST_TOKEN = ROUTE + "RequestToken";
            public const string TOKEN_VERIFIER = ROUTE + "AuthorizeToken?token={0}&isAuthorized=true";
            public const string ACCESS_TOKEN = ROUTE + "AccessToken";
        }
    }

    [TestFixture]
    public class OAuth1Test
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(Oauth1Test));
                                   
        public const string RETURN_URL = "https://api.pps.io/oauth/2/callback";

        [Test]
        public void ThreeLegged_Success()
        {
            GetThreeLegAccessToken(TestCreds.Dan.Consumer, TestCreds.Dan.User, "oob");
        }

        [Test]
        public void ThreeLegged_Success2()
        {
            GetThreeLegAccessToken(TestCreds.Dan.Consumer, TestCreds.Dan.User, "oob");
        }

        [Test]
        public void ThreeLegged_Success3()
        {
            GetThreeLegAccessToken(TestCreds.Dan.Consumer, TestCreds.Dan.User, "oob");
        }
        
        public static void GetThreeLegAccessToken(Creds consumer, Creds user, string returnUrl)
        {
            // Act
            var requestToken = OAuth1Helper.GetRequstToken(consumer, returnUrl);
            var verifier = OAuth1Helper.GetTokenVerifier(requestToken.Key, user);
            var accessToken = OAuth1Helper.GetAccessToken(requestToken.Key, verifier, consumer);

            // Assert
            Assert.IsNotNull(accessToken, "AccessToken");

        }

        public static void GetTwoLegAccessToken(Creds user, string returnUrl)
        {
            GetThreeLegAccessToken(user, user, returnUrl);
        }




        //[Test]
        //public void ThreeLegged_BadMerchant()
        //{
        //    var rt1 = GetRequstToken();
        //    Assert.IsNotNull(rt1);
        //    var rt = GetRequstToken(base.consumerKey, base.consumerSecret);
        //    Assert.IsNotNull(rt);
        //    var ov = GetTokenVerifier(rt, rt1["oauth_token"], "100001320");
        //    Assert.IsNull(ov);

        //}

        //[Test]
        //public void ThreeLegged_DuplicateAuthorization()
        //{
        //    var rt1 = GetRequstToken();
        //    Assert.IsNotNull(rt1);
        //    var rt = GetRequstToken(base.consumerKey, base.consumerSecret);
        //    Assert.IsNotNull(rt);
        //    var ov = GetTokenVerifier(rt, rt1["oauth_token"], "48200");
        //    Assert.IsNotNull(ov);

        //    ov = GetTokenVerifier(rt, rt1["oauth_token"], "48200");
        //    Assert.IsNull(ov);
        //}

        //[Test]
        //public void ThreeLegged_DuplicateAccessToken()
        //{
        //    var rt1 = GetRequstToken();
        //    Assert.IsNotNull(rt1);
        //    var rt = GetRequstToken(base.consumerKey, base.consumerSecret);
        //    Assert.IsNotNull(rt);
        //    var ov = GetTokenVerifier(rt, rt1["oauth_token"], "48200");
        //    Assert.IsNotNull(ov);
        //    var at = GetAccessToken(rt1, ov);
        //    Assert.IsNotNull(at);
        //    at = GetAccessToken(rt1, ov);
        //    Assert.IsNull(at);

        //}

        //[Test]
        //public void OAuth_2()
        //{
        //    var authorizer_RT = GetRequstToken();
        //    Assert.IsNotNull(authorizer_RT);
        //    var authorizer_AT = GetAccessToken(authorizer_RT, null);
        //    Assert.IsNotNull(authorizer_AT);

        //    var authorizer_AT2 = GetAccessToken(authorizer_RT, null);
        //    Assert.IsNull(authorizer_AT2);


        //    var AC = GetAuthorizationCode(authorizer_AT, base.consumerKey, base.consumerSecret);
        //    Assert.IsNotNull(AC);

        //    var AT = GetOAuth2AccessToken(AC);
        //    Assert.IsNotNull(AT);

        //    var AT_bad = GetOAuth2AccessToken(AC);
        //    Assert.IsNull(AT_bad);

        //    var AT2 = GetOAuth2AccessToken((string) (AT as Dictionary<string, object>)["refresh_token"]);
        //    Assert.IsNotNull(AT2);

        //    var AT3 = GetOAuth2AccessToken((string) (AT as Dictionary<string, object>)["refresh_token"]);
        //    Assert.IsNotNull(AT3);
        //    //var at = GetAccessToken(rt1, ov);

        //    //Assert.IsNotNull(at);

        //}
    }


}
