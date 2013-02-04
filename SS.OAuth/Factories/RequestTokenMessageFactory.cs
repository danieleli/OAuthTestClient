using System;
using System.Net.Http;
using SS.OAuth.Extensions;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Factories
{
    [Obsolete]
    public class RequestTokenMessageFactory
    {
        private readonly RequestTokenParams _requestParam;

        public RequestTokenMessageFactory( RequestTokenParams requestParam )
        {
            _requestParam = requestParam;

        }

        public HttpRequestMessage CreateMessage()
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, OAuth.V1.Routes.RequestToken);
            var sig = GetSignature(msg);
            var oauthHeader = CreateHeader(sig);

            msg.Headers.Add((string)OAuth.V1.AUTHORIZATION_HEADER, oauthHeader);

            return msg;
        }

        private string CreateHeader( string sig )
        {
            var oauthHeaderValues = _requestParam.ToCollection();
            oauthHeaderValues.Add(OAuth.V1.Keys.SIGNATURE, sig);
            var headString = oauthHeaderValues.Stringify();
            return "OAuth " + headString;
        }


        private string GetSignature( HttpRequestMessage msg )
        {
            var sigFactory = new SignatureFactory(_requestParam);
            var sig = sigFactory.GetSignature(msg);
            return sig;
        }
    }
}
