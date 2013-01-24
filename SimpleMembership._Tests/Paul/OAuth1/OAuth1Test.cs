#region

using System.Runtime.Remoting;
using MXM.API.Test.Controllers;
using NUnit.Framework;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1
{
    [TestFixture]
    public class OAuth1CryptoFixture
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(OAuth1CryptoFixture));


        /*
           For example, the HTTP request:

             POST /request?b5=%3D%253D&a3=a&c%40=&a2=r%20b HTTP/1.1
             Host: example.com
             Content-Type: application/x-www-form-urlencoded
             Authorization: OAuth realm="Example",
                            oauth_consumer_key="9djdj82h48djs9d2",
                            oauth_token="kkk9d7dh3k39sjv7",
                            oauth_signature_method="HMAC-SHA1",
                            oauth_timestamp="137131201",
                            oauth_nonce="7d8f3e4a",
                            oauth_signature="bYT5CMsGcbgUdFHObYMEfcx6bsw%3D"

             c2&a3=2+q
        */
        

        [Test]
        public void GetRequestToken()
        {
            // Act
            var requestToken = OAuth1Helper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Assert
            Assert.IsNotNull(requestToken, "RequestToken");
            Assert.IsNotNullOrEmpty(requestToken.Key, "RequestToken.Key");
            Assert.IsNotNullOrEmpty(requestToken.Secret, "RequestToken.Token");
        }

        [Test] // Three leg has different consumer and user creds.
        public void ThreeLegged_Success()
        {
            // Act
            var accessToken = GetThreeLegAccessToken(TestCreds.Dan.Consumer, TestCreds.Dan.User, "oob");

            // Assert
            Assert.IsNotNull(accessToken, "AccessToken");
            Assert.IsNotNullOrEmpty(accessToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(accessToken.Secret, "oauth_token_secret");
        }

        [Test] // Two leg is same as three leg but reuse consumer creds as user creds.
        public void TwoLegged_Success()
        {
            // Act            
            var accessToken = GetThreeLegAccessToken(TestCreds.TwoLegUser, TestCreds.TwoLegUser, "oob");

            // Assert
            Assert.IsNotNull(accessToken, "AccessToken");
            Assert.IsNotNullOrEmpty(accessToken.Key, "oauth_token");
            Assert.IsNotNullOrEmpty(accessToken.Secret, "oauth_token_secret");
        }


        [Test, ExpectedException(ExpectedException = typeof(ServerException), ExpectedMessage = "something like: Auth header missing")]
        public void MissingAuthHeader_Should_ThrowServerException_On_RequestToken()
        {
            // Act
            var requestToken = OAuth1Helper.GetRequstToken(TestCreds.Dan.Consumer, "oob");

            // Assert
            Assert.Fail("Expected ServerException not thrown.");
        }


        [Test, ExpectedException(ExpectedException = typeof(ServerException))]
        public void BadVerifier_Should_ThrowServerException_On_RequestVerifier()
        {
            // Arrrange
            var requestToken = OAuth1Helper.GetRequstToken(TestCreds.ThreeLegConsumer, "oob");

            // Act
            var verifier = OAuth1Helper.GetTokenVerifier(requestToken.Key, TestCreds.ThreeLegUser);

            // Assert
            Assert.Fail("Expected ServerException not thrown.");
        }


        public static Creds GetThreeLegAccessToken(Creds consumer, Creds user, string returnUrl)
        {
            var requestToken = OAuth1Helper.GetRequstToken(consumer, returnUrl);
            var verifier = OAuth1Helper.GetTokenVerifier(requestToken.Key, user);
            var accessToken = OAuth1Helper.GetAccessToken(requestToken.Key, verifier, consumer);

            return accessToken;
        }

        public static void GetTwoLegAccessToken(Creds user, string returnUrl)
        {
            GetThreeLegAccessToken(user, user, returnUrl);
        }

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