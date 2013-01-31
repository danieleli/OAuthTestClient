using System;
using System.Collections.Specialized;
using System.Net.Http;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;

namespace SS.OAuth1.Client._Tests.Helpers
{
    public class TestParameter : OAuthParametersBase
    {
        public TestParameter(Creds consumer, HttpMethod method, string url, string authority=null) : base(consumer, method, url, authority) {}


        public override NameValueCollection GetOAuthParams()
        {
            return base.GetOAuthParamsCore();
        }

        public override string GetOAuthHeader()
        {
            return OAuthParser.CreateHeader(this, null);
        }
    }
}