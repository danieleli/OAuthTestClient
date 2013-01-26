using System.Net.Http;
using SimpleMembership._Tests.Paul.Helpers;

namespace SimpleMembership._Tests.Paul.OAuth1.Crypto
{
    /// <summary>
    /// Access Token
    /// </summary>
    public static class AccessTokenSigner
    {
        public static HttpRequestMessage Sign(HttpRequestMessage msg, Creds consumer, Creds verifierToken)
        {
            var timestamp = OAuthUtils.GenerateTimeStamp();
            var nonce = OAuthUtils.GenerateNonce();


            var signature = Signature.GetOAuth1ASignature(msg.RequestUri, msg.Method, consumer.Key, verifierToken.Secret,
                                                          verifierToken.Key, null, timestamp, nonce,
                                                          null, verifierToken.Secret);

            var authHeader = CryptoHelper.GetAuthHeader(consumer, null, nonce, timestamp, signature, verifierToken.Key);

            msg.Headers.Add("Authorization", authHeader);
            return msg;
        }
    }
}