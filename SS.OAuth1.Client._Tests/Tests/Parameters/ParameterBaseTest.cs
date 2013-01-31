using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;
using SS.OAuth1.Client._Tests.Tests.Helpers;
using log4net;

namespace SS.OAuth1.Client._Tests.Tests.Parameters
{
    public class TestParameter : OAuthParametersBase
    {
        public TestParameter(Creds consumer, HttpMethod method, string url, string authority=null) : base(consumer, method, url, authority)
        {
        }

        public override string GetOAuthHeader()
        {
            throw new NotImplementedException();
        }

        public string GetSignatureBase()
        {
            return this.OAuthParser.GetSignatureBase(this);
        }
    }

    [TestFixture]
    public class ParameterBaseTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(ParameterBaseTest));

        [Test]
        public void GetSignatureBase()
        {
            // Arrange
            const string cKey = "consumerKey";
            const string cSecret = "consumerSecret";
            const string url = "HTTPS://www.ExAMplwwwe.com/Auth?a=valuea&z=valuez";
            const string authority = "authority";
            var consumer = new Creds(cKey, cSecret);
            var method = HttpMethod.Get;
            

            var testParam = new TestParameter(consumer, method, url, authority);

            var sigBase = testParam.GetSignatureBase();
            LOG.Debug("SigBase: " + sigBase);

        }
    }
}
