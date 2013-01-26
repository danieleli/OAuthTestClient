using System.Net.Http;
using SimpleMembership._Tests.Paul.Helpers;

namespace SimpleMembership._Tests.Paul.OAuth1.Crypto
{
    public static class RequestTokenSigner
    {
        public static HttpRequestMessage Sign(HttpRequestMessage msg, Creds consumer, string callback = "oob")
        {
            var timestamp = OAuthUtils.GenerateTimeStamp();
            var nonce = OAuthUtils.GenerateNonce();

            var signature = Signature.GetOAuth1ASignature(msg.RequestUri, msg.Method, consumer.Key, consumer.Secret,
                                                          null, null, timestamp, nonce,
                                                          callback, null);

            var authHeader = CryptoHelper.GetAuthHeader(consumer, callback, nonce, timestamp, signature);

            msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, authHeader);
            return msg;
        }
    }
}