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

        public SignatureBaseStringFactory(BaseParams paramz)
        {
            _paramz = paramz;
        }

        public string GetSignatureBase( HttpRequestMessage msg )
        {
            var method          = msg.Method.ToString().ToUpper();
            var baseUri         = msg.RequestUri.GetBaseStringUri().UrlEncodeForOAuth();
            var paramCollection = GetAllRequestParameters(msg);
            var paramz          = paramCollection.Normalize().UrlEncodeForOAuth();                                                       

            var rtn = string.Format("{0}&{1}&{2}", method, baseUri, paramz);

            return rtn;
        }

        public NameValueCollection GetAllRequestParameters( HttpRequestMessage msg )
        {
            var oauthParams       = _paramz.ToCollection();
            var queryParams       = msg.RequestUri.ParseQueryString();
            var contentCollection = GetRequestContent( msg );

            var rtnCollection = new NameValueCollection {oauthParams, queryParams, contentCollection};

            return rtnCollection;
        }

        private NameValueCollection GetRequestContent( HttpRequestMessage msg )
        {
            var contentCollection = new NameValueCollection();
            if (msg.Content != null)
            {
                var contentAsString = msg.Content.ReadAsStringAsync().Result;
                contentCollection = HttpUtility.ParseQueryString(contentAsString);
            }
            return contentCollection;
        }
    }

}
