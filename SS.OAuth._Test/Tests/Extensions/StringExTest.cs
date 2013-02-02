using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SS.OAuth.Extensions;
using log4net;

namespace SS.OAuth.Tests.Extensions
{
    [TestFixture]
    public class StringExTest
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(StringExTest));

        [Test]
        public void UrlEncode_Slash()
        {
            var result = "/".UrlEncodeForOAuth();
            LOG.Debug("Slash: " + result);

            result = "/7".UrlEncodeForOAuth();
            LOG.Debug("Slash7: " + result);

            result = "zK2rxSJ5P2eCTUDPg/7NxzKB+uk=".UrlEncodeForOAuth();
            LOG.Debug("Sample Sig: " + result);
        }
    }

}
