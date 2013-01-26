#region

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using SimpleMembership._Tests.Paul.Helpers;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1
{
    /// <summary>
    ///     http://tools.ietf.org/html/rfc5849#section-3.4.1.1
    /// </summary>
    public static class Crypto
    {
        public const string AUTHORIZATION_HEADER = "Authorization";
        private static readonly ILog LOG = LogManager.GetLogger(typeof (Crypto));

        public static string GetAuthHeader(Creds consumer, string callback, string nonce, string timestamp,
                                            string signature, string token = null, string verifier = null)
        {
            var oauthParams = OAuth.V1.GetOAuthParams(callback, consumer.Key, nonce, null, timestamp, token, null,
                                                      verifier);
            oauthParams.Add(OAuth.V1.Keys.SIGNATURE, signature);

            var authHeader = "OAuth " + Stringify(oauthParams);

            LOG.Debug("Authorization Header: " + authHeader);
            return authHeader;
        }

        public static string Stringify(SortedDictionary<string, string> paramz)
        {
            var sb = new StringBuilder();
            var isFirstItem = true;
            foreach (var p in paramz)
            {
                if (!isFirstItem)
                {
                    sb.Append(",");
                }
                var key = Uri.EscapeDataString(p.Key);
                var value = Uri.EscapeDataString(p.Value);
                sb.Append(string.Format("{0}=\"{1}\"", key, value));
                isFirstItem = false;
            }

            return sb.ToString();
        }

        /// <summary>
        /// Request Token
        /// </summary>
        public static class RequestTokenSigner
        {
            public static HttpRequestMessage Sign(HttpRequestMessage msg, Creds consumer, string callback)
            {
                var timestamp = OAuthUtils.GenerateTimeStamp();
                var nonce = OAuthUtils.GenerateNonce();

                var signature = Signature.GetOAuth1ASignature(msg.RequestUri, msg.Method, consumer.Key, consumer.Secret,
                                                              null, null, timestamp, nonce,
                                                              callback, null);

                var authHeader = GetAuthHeader(consumer, callback, nonce, timestamp, signature);

                msg.Headers.Add(AUTHORIZATION_HEADER, authHeader);
                return msg;
            }
        }


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

                var authHeader = GetAuthHeader(consumer, null, nonce, timestamp, signature);

                msg.Headers.Add("Authorization", authHeader);
                return msg;
            }
        }


        /// <summary>
        /// Access Token
        /// </summary>
        public static class AccessTokenSigner
        {
            public static HttpRequestMessage Sign(HttpRequestMessage msg, Creds consumer, Creds verifierToken)
            {
                var timestamp = OAuthUtils.GenerateTimeStamp();
                var nonce = OAuthUtils.GenerateNonce();

                var signature = Signature.GetOAuth1ASignature(msg.RequestUri, msg.Method, consumer.Key, consumer.Secret,
                                                              verifierToken.Key, null, timestamp, nonce,
                                                              null, verifierToken.Secret);

                var authHeader = GetAuthHeader(consumer, null, nonce, timestamp, signature, verifierToken.Key);

                msg.Headers.Add("Authorization", authHeader);
                return msg;
            }
        }
    }
}