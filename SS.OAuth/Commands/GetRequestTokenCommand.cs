using System;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using SS.OAuth.Extensions;
using SS.OAuth.Factories;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;
using log4net;

namespace SS.OAuth.Commands
{
    public class GetRequestTokenCommand
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(GetRequestTokenCommand));

        private readonly RequestTokenParams _paramz;
        private readonly HttpClient _httpClient = new HttpClient();

        public GetRequestTokenCommand( RequestTokenParams paramz )
        {
            _paramz = paramz;
        }

        public Creds GetToken()
        {
            var msg = this.CreateMessage();
            var sig = this.GetSignature(msg);
            var header = this.CreateHeader(sig);
            msg.Headers.Add((string)OAuth.V1.AUTHORIZATION_HEADER, header);
            var response = _httpClient.SendAsync(msg).Result;
            var token = ExtractToken(response);

            return token;
        }

        private string CreateHeader( string sig )
        {
            var oauthHeaderValues = _paramz.ToCollection();
            oauthHeaderValues.Add(OAuth.V1.Keys.SIGNATURE, sig);
            var headString = oauthHeaderValues.Stringify();
            return "OAuth " + headString;
        }

        private HttpRequestMessage CreateMessage()
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, OAuth.V1.Routes.RequestToken);
            var mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            return msg;
        }

        private string GetSignature( HttpRequestMessage msg )
        {
            var sigFactory = new SignatureFactory(_paramz, msg);
            var sig = sigFactory.GetSignature();
            return sig;
        }

        private Creds ExtractToken( HttpResponseMessage response )
        {
            var result = response.Content.ReadAsFormDataAsync().Result;

            if (result == null) throw new Exception("No content found.");

            var key = result[OAuth.V1.Keys.TOKEN];
            var secret = result[OAuth.V1.Keys.TOKEN_SECRET];
            var token = new Creds(key, secret);

            return token;
        }
    }
}