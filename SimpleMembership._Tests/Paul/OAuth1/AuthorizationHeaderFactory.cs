#region

using System;
using System.Collections.Generic;
using System.Text;
using SimpleMembership._Tests.Paul.Helpers;
using log4net;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1
{
    public static class AuthorizationHeaderFactory
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof (AuthorizationHeaderFactory));

        public static string CreateRequestTokenHeader(RequestTokenInput input)
        {
            var signature = Signature.GetOAuth1ASignature(input.RequestUri, input.HttpMethod, input.Consumer.Key,
                                                          input.Consumer.Secret,
                                                          null, null, input.Timestamp, input.Nonce,
                                                          input.Callback, null);

            var oauthParams = OAuth.V1.GetOAuthParams(input.Callback, input.Consumer.Key, input.Nonce, signature,
                                                      input.Timestamp);
            var header = "OAuth " + Stringify(oauthParams);

            return header;
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

        public static string CreateVerifierHeader()
        {
            return "";
        }

        public static string CreateAccessTokenHeader()
        {
            //var signature = Signature.GetOAuth1ASignature(msg.RequestUri, msg.Method, consumer.Key, verifierToken.Secret,
            //                                              verifierToken.Key, null, timestamp, nonce,
            //                                              null, verifierToken.Secret);

            //var oauthParams = OAuth.V1.GetOAuthParams(input.Callback, input.Consumer.Key, input.Nonce, null, input.Timestamp);
            //oauthParams.Add(OAuth.V1.Keys.SIGNATURE, "")//signature);

            //var header = "OAuth " + CryptoHelper.Stringify(oauthParams);

            //return header;

            return "";
        }
    }
}