using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using SS.OAuth.Extensions;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Models.Requests
{
    public class RequestTokenRequest
    {
        private readonly RequestTokenParameters _paramz;
        private readonly HttpRequestMessage _msg;

        public RequestTokenRequest(RequestTokenParameters paramz, HttpRequestMessage msg)
        {
            _paramz = paramz;
            _msg = msg;
        }

        public string GetSignatureBase()
        {
            var method = _msg.Method.ToString().ToUpper();
            var baseUri = _msg.RequestUri.GetBaseStringUri().UrlEncodeForOAuth();
            var paramz = GetAllRequestParameters();

            var rtn = string.Format("{0}&{1}&{2}", method, baseUri, paramz);

            return rtn;
        }

        private NameValueCollection GetAllRequestParameters()
        {
            
            var rtnCollection = new NameValueCollection();
            var oauthParams = _paramz.GetOAuthParams();
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
    }
}
