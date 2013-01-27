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

        public static string CreateRequestTokenHeader(RequestTokenParameters parameters)
        {
            var signature = Signature.GetOAuth1ASignature(parameters.RequestUri, parameters.HttpMethod, parameters.Consumer.Key,
                                                          parameters.Consumer.Secret,
                                                          null, null, parameters.Timestamp, parameters.Nonce,
                                                          parameters.Callback, null);

            var oauthParams = AuthParameterFactory.GetOAuthParams(parameters.Consumer.Key, parameters.Nonce, signature,
                                                      parameters.Timestamp, parameters.Callback);
            var header = "OAuth " + Stringify(oauthParams);

            return header;
        }

        public static string CreateVerifierHeader()
        {
            throw new NotImplementedException();
        }

        public static string CreateAccessTokenHeader(AccessTokenParameters parameters)
        {

            var signature = Signature.GetOAuth1ASignature(parameters.RequestUri, parameters.HttpMethod, parameters.Consumer.Key,
                                                          parameters.Consumer.Secret, null, null, parameters.Timestamp,
                                                          parameters.Nonce, "", parameters.Verifier);

            var oauthParams = AuthParameterFactory.GetOAuthParams(parameters.Consumer.Key, parameters.Nonce, signature, parameters.Timestamp, "",
                                                      "", "", parameters.Verifier);


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