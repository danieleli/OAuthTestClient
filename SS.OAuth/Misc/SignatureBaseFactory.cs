using System;
using System.Collections.Specialized;
using System.Net.Http;
using SS.OAuth.Extensions;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Misc
{
    public class SignatureBaseFactory
    {
        private readonly RequestTokenParameters _paramz;
        private readonly HttpMethod _method;
        private readonly Uri _uri;

        public SignatureBaseFactory(RequestTokenParameters paramz, HttpMethod method, Uri uri)
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

            var queryParams = _uri.ParseQueryString();
            rtnCollection.Add(queryParams);

            return rtnCollection;
        }




    }
}
