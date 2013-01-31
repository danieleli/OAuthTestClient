using System;
using System.Net.Http;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;

namespace SS.OAuth1.Client._Tests.Helpers
{
    public class TestParameter : OAuthParametersBase
    {
        public TestParameter(Creds consumer, HttpMethod method, string url, string authority=null) : base(consumer, method, url, authority) {}

        public override string GetOAuthHeader()
        {
            throw new NotImplementedException();
        }

        public string GetSignatureBase()
        {
            return this.OAuthParser.GetSignatureBase(this);
        }
    }
}