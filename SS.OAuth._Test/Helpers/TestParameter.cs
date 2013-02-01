using System.Collections.Specialized;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Helpers
{
    public class TestParameter : BaseParameter
    {
        public TestParameter(Creds consumer) : base(consumer) {}


        public override NameValueCollection GetOAuthParams()
        {
            return base.GetOAuthParamsCore();
        }

    }
}