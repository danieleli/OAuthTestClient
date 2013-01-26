#region

using System;
using System.Collections.Generic;
using System.Text;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1.Crypto
{
    /// <summary>
    ///     http://tools.ietf.org/html/rfc5849#section-3.4.1.1
    /// </summary>
    public static class CryptoHelper
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(CryptoHelper));

        public static string GetAuthHeader(Creds consumer, string callback, string nonce, string timestamp,
                                            string signature, string token = null, string verifier = null)
        {
            var oauthParams = OAuth.V1.GetOAuthParams(callback, consumer.Key, nonce, null, timestamp, token, null,
                                                      verifier);
            oauthParams.Add(OAuth.V1.Keys.SIGNATURE, signature);

            var authHeader = "OAuth " + Stringify(oauthParams);
            LOG.Debug("Authorization Header: \n\n\t" + authHeader.Replace(",", "\n\t") + "\n");
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

}
}