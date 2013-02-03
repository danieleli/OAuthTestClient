using System.Collections.Specialized;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth.Factories;
using SS.OAuth.Helpers;
using SS.OAuth.Models;
using SS.OAuth.Extensions;
using log4net;

namespace SS.OAuth.Tests.Requests
{
    [TestFixture]
    public class RequestTokenTest
    {
        static readonly ILog LOG = LogManager.GetLogger(typeof(RequestTokenTest));

        #region -- Expected Values --

        // Expected values from:
        // http://oauth.googlecode.com/svn/code/javascript/example/signature.html

        private const string EXPECTED_SIG = "zK2rxSJ5P2eCTUDPg/7NxzKB+uk=";

        private const string EXPECTED_BASE =
            "POST&http%3A%2F%2Fterm.ie%2Foauth%2Fexample%2Frequest_token.php&oauth_consumer_key%3Dkey%26oauth_nonce%3D3937336%26oauth_signature_method%3DHMAC-SHA1%26oauth_timestamp%3D1359756560";

        // "realm" param is optional so I'm leaving it out.
        // Exact original: EXPECTED_HEADER = "OAuth realm=\"\",oauth_consumer_key=\"key\",oauth_timestamp=\"1359756560\",oauth_nonce=\"3937336\",oauth_signature_method=\"HMAC-SHA1\",oauth_signature=\"zK2rxSJ5P2eCTUDPg%2F7NxzKB%2Buk%3D\"";
        private const string EXPECTED_HEADER =
            "OAuth oauth_consumer_key=\"key\",oauth_nonce=\"3937336\",oauth_signature=\"zK2rxSJ5P2eCTUDPg%2F7NxzKB%2Buk%3D\",oauth_signature_method=\"HMAC-SHA1\",oauth_timestamp=\"1359756560\"";

        #endregion

        [Test]
        public void GetSignature()
        {
            // Arrange
            var consumer   = new Creds("key", "secret");
            var param      = new TestParams(consumer, "3937336", "1359756560");
            var msg        = new HttpRequestMessage(HttpMethod.Post, "http://term.ie/oauth/example/request_token.php");
            var sigFactory = new SignatureFactory(param, msg);

            // Act            
            var sigBase      = sigFactory.GetSignatureBase();
            var sigKey       = param.GetSignatureKey();
            var sig          = sigFactory.GetSignature(sigBase, sigKey);
            var headerParams = AddSig(sigFactory, sig);
            var headString   = "OAuth " + headerParams.Stringify();
            
            LogValues(param, sigBase, sigKey, sig, headString);

            // Assert
            Assert.That(sigBase, Is.EqualTo(EXPECTED_BASE), "Signature Base");
            Assert.That(sig, Is.EqualTo(EXPECTED_SIG), "Signature");
            Assert.That(headString, Is.EqualTo(EXPECTED_HEADER), "Header");
        }

        private static NameValueCollection AddSig(SignatureFactory sigFactory, string sig)
        {
            var headerParams = sigFactory.GetOAuthParams();
            headerParams.Add(OAuth.V1.Keys.SIGNATURE, sig);
            return headerParams;
        }

        private static void LogValues(TestParams param, string sigBase, string sigKey, string sig, string headString)
        {
            LOG.Debug("Nonce: " + param.Nonce);
            LOG.Debug("Timestamp: " + param.Timestamp);
            LOG.Debug("Realm: " + param.Realm);
            LOG.Debug("Signature Base: " + sigBase);
            LOG.Debug("Signature Key: " + sigKey);
            LOG.Debug("Signature: " + sig);
            LOG.Debug("Encode Signature (Don't do this.): " + sig.UrlEncodeForOAuth());
            LOG.Debug("Header: " + headString);
        }
    }
}
