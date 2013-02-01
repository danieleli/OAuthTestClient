using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SS.OAuth.Misc;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Tests.Requests
{
    [TestFixture]
    public class RequestToken
    {
        readonly Creds _consumer = new Creds("consumer key", "consumer secret");

        [Test]
        public void GetSignature()
        {
            var param = new RequestTokenParameters(_consumer);

            var sigFactory = new SignatureBaseFactory(param, HttpMethod.Get, new Uri("http://example.com/request_token"));

            var sigBase = sigFactory.GetSignatureBase();
            var sigKey = param.GetSignatureKey();
            var sig = GetSig(sigBase, sigKey);


        }

        private object GetSig(string sigBase, string sigKey)
        {
            throw new NotImplementedException();
        }
    }
}
