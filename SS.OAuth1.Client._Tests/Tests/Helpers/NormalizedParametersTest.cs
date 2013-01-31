using System;
using System.Collections.Specialized;
using System.Net.Http;
using NUnit.Framework;
using SS.OAuth1.Client.Extensions;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client._Tests.Helpers;
using SS.OAuth1.Client._Tests.Tests.Parameters;
using log4net;

namespace SS.OAuth1.Client._Tests.Tests.Helpers
{

    [TestFixture]
    public class NormalizedParametersTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(OAuthParserTest));
        private readonly TestParameter _testParam;
        private readonly OAuthParser _parser;

        public NormalizedParametersTest()
        {
            const string cKey = "consumerKey";
            const string cSecret = "consumerSecret";
            const string url = "HTTPS://www.ExAMplwwwe.com/Auth?a=valuea&z=valuez&b=valueb1&b=valueb2";
            const string authority = "authority";
            var consumer = new Creds(cKey, cSecret);
            var method = HttpMethod.Get;

            _testParam = new TestParameter(consumer, method, url, authority);
            _parser = new OAuthParser();

        }

        [Test]
        public void Include_QueryStringParams()
        {
            var normalizedRequestParams = GetNormalizedRequestParameters(_testParam);

            LOG.Debug("normal params: " + normalizedRequestParams);

            Assert.That(normalizedRequestParams, Is.Not.Null, "normalizedRequestParams");

            var valuesA = normalizedRequestParams.GetValues("a");
            Assert.That(valuesA, Is.Not.Null, "valuesUnderKeyA");
            Assert.That(valuesA.Length, Is.EqualTo(1), "Count of RequestParams Named 'a'");


            var valuesB = normalizedRequestParams.GetValues("b");
            Assert.That(valuesB, Is.Not.Null, "valuesUnderKeyB");
            Assert.That(valuesB.Length, Is.EqualTo(2), "Count of RequestParams Named 'b'");
        }

        [Test]
        public void Includes_ItemWithName_ConsumerKey()
        {
            var normalizedRequestParams = GetNormalizedRequestParameters(_testParam);

            LOG.Debug("normal params: " + normalizedRequestParams);

            var values = normalizedRequestParams.GetValues(Keys.CONSUMER_KEY);
            Assert.That(values, Is.Not.Null, "oauthConsumerTokenKey_Values");
            Assert.That(values.Length, Is.EqualTo(1), "Count of RequestParams Named " + Keys.CONSUMER_KEY);


            LOG.Debug("normal params.Stringify: " + normalizedRequestParams.Stringify());
        }

        [Test]
        public void Includes_EntityBody()
        {
         throw new NotImplementedException();
        }

        public NameValueCollection GetNormalizedRequestParameters(TestParameter p)
        {
            var rtnCollection = p.RequestUri.ParseQueryString();
            
            var oauthParams = p.GetOAuthParams();
            rtnCollection.Add(oauthParams);

            return rtnCollection;
        }
    }
}
