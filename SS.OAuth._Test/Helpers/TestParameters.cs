using System.Collections.Specialized;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Helpers
{
    public class TestParameters : BaseParameters
    {
        public TestParameters(Creds consumer) : base(consumer) {}

    }
}