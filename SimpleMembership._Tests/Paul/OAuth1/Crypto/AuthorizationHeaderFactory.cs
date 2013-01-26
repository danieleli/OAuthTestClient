using System;
using SimpleMembership._Tests.Paul.Helpers;
using log4net;

namespace SimpleMembership._Tests.Paul.OAuth1.Crypto
{
    public static class AuthorizationHeaderFactory
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(AuthorizationHeaderFactory));

        public static string CreateRequestTokenHeader(RequestTokenSignatureInput input)
        {

            var signature = Signature.GetOAuth1ASignature(input.RequestUri, input.HttpMethod, input.Consumer.Key, input.Consumer.Secret,
                                                          null, null, input.Timestamp, input.Nonce,
                                                          input.Callback, null);

            var oauthParams = OAuth.V1.GetOAuthParams(input.Callback, input.Consumer.Key, input.Nonce, null, input.Timestamp);
            oauthParams.Add(OAuth.V1.Keys.SIGNATURE, signature);

            var header = "OAuth " + CryptoHelper.Stringify(oauthParams);

            return header;
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

        public static string CreateVerifierHeader()
        {
            return "";
        }
    }
}