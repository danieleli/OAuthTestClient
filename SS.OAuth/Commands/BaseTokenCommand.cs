using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using SS.OAuth.Extensions;
using SS.OAuth.Factories;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Commands
{
    public class BaseTokenCommand
    {
        protected BaseParams _paramz;
        
        protected string CreateHeader( string sig )
        {
            var oauthHeaderValues = _paramz.ToCollection();
            oauthHeaderValues.Add(OAuth.V1.Keys.SIGNATURE, sig);
            var headString = oauthHeaderValues.Stringify();
            return "OAuth " + headString;
        }

        

        protected string GetSignature( HttpRequestMessage msg )
        {
            var sigFactory = new SignatureFactory(_paramz, msg);
            var sig = sigFactory.GetSignature();
            return sig;
        }

        protected Creds ExtractToken( HttpResponseMessage response )
        {
            var result = response.Content.ReadAsFormDataAsync().Result;

            if (result == null) throw new Exception("No content found.");

            var key = result[OAuth.V1.Keys.TOKEN];
            var secret = result[OAuth.V1.Keys.TOKEN_SECRET];
            var token = new Creds(key, secret);

            return token;
        }
    }
}
