using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using SS.OAuth.Extensions;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Factories
{
    /* 3.4.2.  HMAC-SHA1
    
       The "HMAC-SHA1" signature method uses the HMAC-SHA1 signature
       algorithm as defined in [RFC2104]:

         digest = HMAC-SHA1 (key, text)

       The HMAC-SHA1 function variables are used in following way:

       text    is set to the value of the signature base string from
               Section 3.4.1.1.

       key     is set to the concatenated values of:

               1.  The client shared-secret, after being encoded
                   (Section 3.6).

               2.  An "&" character (ASCII code 38), which MUST be included
                   even when either secret is empty.

               3.  The token shared-secret, after being encoded
                   (Section 3.6).

       digest  is used to set the value of the "oauth_signature" protocol
               parameter, after the result octet string is base64-encoded
               per [RFC2045], Section 6.8.
    */

    public class SignatureFactory
    {
        private readonly BaseParams _paramz;
        private readonly HttpMethod _method;
        private readonly Uri _uri;

        public SignatureFactory(BaseParams paramz, HttpRequestMessage httpMessage)
        {
            
            _paramz = paramz;
            _method = httpMessage.Method;
            _uri = httpMessage.RequestUri;
        }

        public string GetSignature(string sigBase, string sigKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(sigKey);
            var sha1 = new HMACSHA1(keyBytes);

            var baseBtyes = Encoding.UTF8.GetBytes(sigBase);
            var hash = sha1.ComputeHash(baseBtyes);

            return Convert.ToBase64String(hash);
        }


        public string GetSignatureBase()
        {
            var method = _method.ToString().ToUpper();
            var baseUri = _uri.GetBaseStringUri().UrlEncodeForOAuth();
            var paramz = GetAllRequestParameters().Normalize().UrlEncodeForOAuth();

            var rtn = string.Format("{0}&{1}&{2}", method, baseUri, paramz);

            return rtn;
        }

        public NameValueCollection GetAllRequestParameters()
        {
            var rtnCollection = new NameValueCollection();

            var oauthParams = GetOAuthParams();
            rtnCollection.Add(oauthParams);

            var queryParams = _uri.ParseQueryString();
            rtnCollection.Add(queryParams);

            return rtnCollection;
        }
           
        public NameValueCollection GetOAuthParams()
        {
            var factory = new HeaderFactory();
            var col = factory.GetOAuthParams(_paramz);

            return col;
        }
    }
}
