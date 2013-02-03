using System.Collections.Specialized;
using System.Net.Http;
using System.Web;
using SS.OAuth.Extensions;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Factories
{
    public class SignatureBaseStringFactory
    {
        private readonly BaseParams _paramz;
        private readonly HttpRequestMessage _msg;

        public SignatureBaseStringFactory(BaseParams paramz, HttpRequestMessage msg)
        {
            _paramz = paramz;
            _msg = msg;
        }

        public string GetSignatureBase()
        {
            var method          = _msg.Method.ToString().ToUpper();
            var baseUri         = _msg.RequestUri.GetBaseStringUri().UrlEncodeForOAuth();
            var paramCollection = GetAllRequestParameters();
            var paramz          = paramCollection.Normalize().UrlEncodeForOAuth();                                                       

            var rtn = string.Format("{0}&{1}&{2}", method, baseUri, paramz);

            return rtn;
        }

        public NameValueCollection GetAllRequestParameters()
        {
            var oauthParams       = _paramz.ToCollection();
            var queryParams       = _msg.RequestUri.ParseQueryString();
            var contentCollection = GetRequestContent();

            var rtnCollection = new NameValueCollection {oauthParams, queryParams, contentCollection};

            return rtnCollection;
        }

        private NameValueCollection GetRequestContent()
        {
            var contentCollection = new NameValueCollection();
            if (_msg.Content != null)
            {
                var contentAsString =_msg.Content.ReadAsStringAsync().Result;
                contentCollection = HttpUtility.ParseQueryString(contentAsString);
            }
            return contentCollection;
        }
    }

}
