using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using SS.OAuth.Extensions;
using SS.OAuth.Models.Parameters;
using log4net;

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
        private static readonly ILog LOG = LogManager.GetLogger(typeof(SignatureFactory));
        
        private readonly BaseParams _paramz;

        public SignatureBaseStringFactory SignatureBaseStringFactory { get; private set; }
        
        public SignatureFactory(BaseParams paramz)
        {
            _paramz = paramz;
            SignatureBaseStringFactory = new SignatureBaseStringFactory(_paramz);
        }

        public string GetSignature(HttpRequestMessage msg)
        {
            var sigBase = SignatureBaseStringFactory.GetSignatureBase(msg);
            var sigKey = _paramz.GetSignatureKey();
            var sig = GetSignature(sigBase, sigKey);

            LogInfo(sigBase, sigKey, sig);
            return sig;
        }

        public string CreateHeader( string sig )
        {
            var oauthHeaderValues = _paramz.ToCollection();
            oauthHeaderValues.Add(OAuth.V1.Keys.SIGNATURE, sig);
            var headString = oauthHeaderValues.Stringify();
            return "OAuth " + headString;
        }

        private static void LogInfo(string sigBase, string sigKey, string sig)
        {
            LOG.Info("");
            LOG.Info("Signature Base  : " + sigBase);
            LOG.Info("Signature Key   : " + sigKey);
            LOG.Info("Signature Value : " + sig);
            LOG.Info("");
        }

        private static string GetSignature(string sigBase, string sigKey)
        {
            var keyBytes = Encoding.UTF8.GetBytes(sigKey);
            var sha1 = new HMACSHA1(keyBytes);

            var baseBtyes = Encoding.UTF8.GetBytes(sigBase);
            var hash = sha1.ComputeHash(baseBtyes);

            return Convert.ToBase64String(hash);
        }
    }
}
