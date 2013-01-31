using System.Net.Http;
using NUnit.Framework;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client._Tests.Helpers;
using log4net;

namespace SS.OAuth1.Client._Tests.Tests.Parameters
{
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


  
    }
}
