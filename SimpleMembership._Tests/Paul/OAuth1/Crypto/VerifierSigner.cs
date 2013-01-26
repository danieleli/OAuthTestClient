using System.Net.Http;
using SimpleMembership._Tests.Paul.Helpers;

namespace SimpleMembership._Tests.Paul.OAuth1.Crypto
{
    /// <summary>
    /// Verifier Token
    /// </summary>
    public static class VerifierSigner
    {
        public static HttpRequestMessage Sign(HttpRequestMessage msg, Creds consumer, Creds requestToken)
        {
            var timestamp = OAuthUtils.GenerateTimeStamp();
            var nonce = OAuthUtils.GenerateNonce();

            var signature = Signature.GetOAuth1ASignature(msg.RequestUri, msg.Method, consumer.Key, consumer.Secret,
                                                          requestToken.Key, requestToken.Secret, timestamp, nonce,
                                                          null, null);

            var authHeader = CryptoHelper.GetAuthHeader(consumer, null, nonce, timestamp, signature);

            msg.Headers.Add("Authorization", authHeader);
            return msg;
        }
    }
}