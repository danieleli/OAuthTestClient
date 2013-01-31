using System;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Parameters;
using log4net;

namespace SS.OAuth1.Client._Tests.Tests.Parameters
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

    [TestFixture]
    public class ParameterBaseTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(ParameterBaseTest));
        private readonly TestParameter _testParam = null;

        public ParameterBaseTest()
        {
            // Arrange
            const string cKey = "consumerKey";
            const string cSecret = "consumerSecret";
            const string url = "HTTPS://www.ExAMplwwwe.com/Auth?a=valuea&z=valuez&b=valueb1&b=valueb2";
            const string authority = "authority";
            var consumer = new Creds(cKey, cSecret);
            var method = HttpMethod.Get;
            
            _testParam = new TestParameter(consumer, method, url, authority);

        }
        
        [Test]
        public void GetSignatureBase()
        {
            var sigBase = _testParam.GetSignatureBase();
            LOG.Debug("SigBase: " + sigBase);

        }

  
    }
}
