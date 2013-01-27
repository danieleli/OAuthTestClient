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

            var oauthParams = OAuth.V1.GetOAuthParams(input.Consumer.Key, input.Nonce, signature,
                                                      input.Timestamp, input.Callback);
            var header = "OAuth " + Stringify(oauthParams);

            return header;
        }

        public static string CreateVerifierHeader()
        {
            throw new NotImplementedException();
        }

        public static string CreateAccessTokenHeader(AccessTokenInput input)
        {

            var signature = Signature.GetOAuth1ASignature(input.RequestUri, input.HttpMethod, input.Consumer.Key,
                                                          input.Consumer.Secret, null, null, input.Timestamp,
                                                          input.Nonce, "", input.Verifier);

            var oauthParams = OAuth.V1.GetOAuthParams(input.Consumer.Key, input.Nonce, signature, input.Timestamp, "",
                                                      "", "", input.Verifier);


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
                var key = Uri.EscapeUriString(p.Key);
                var value = Uri.EscapeUriString(p.Value);
                sb.Append(string.Format("{0}=\"{1}\"", key, value));
                isFirstItem = false;
            }

            return sb.ToString();
        }
    }
}