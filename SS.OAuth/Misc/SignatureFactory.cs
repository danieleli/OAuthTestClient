using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SS.OAuth.Extensions;
using SS.OAuth.Misc;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Models.Requests
{
    public class SignatureFactory
    {
        private readonly RequestTokenParameters _paramz;
        private readonly HttpMethod _method;
        private readonly Uri _uri;

        public SignatureFactory(RequestTokenParameters paramz, HttpMethod method, Uri uri)
        {
            _paramz = paramz;
            _method = method;
            _uri = uri;
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
            var oauthParams = new OAuthCollection(_paramz);

            rtnCollection.Add(oauthParams);

            //httpContent = _msg.Content;
            //if (httpContent != null)
            //{
            //    rtnCollection.Add(httpContent);
            //}

            //var queryParams = _msg.RequestUri.ParseQueryString();
            //rtnCollection.Add(queryParams);

            return rtnCollection;
        }

        public string GetOAuthHeader()
        {
            var sig = GetSignature();
            var col = new OAuthCollection(_paramz) {{V1.Keys.SIGNATURE, sig}};

            return "OAuth " + col.Stringify();
        }

        public string GetSignature()
        {
            var sigBase = GetSignatureBase();
            var sigKey = _paramz.GetSignatureKey();
            var sig = GetSig(sigBase, sigKey);

            return sig;
        }

        public string GetSig(string sigBase, string sigKey)
        {
            throw new NotImplementedException();
        }


    }
}
