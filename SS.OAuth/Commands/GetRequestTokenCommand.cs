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
    public class GetRequestTokenCommand: BaseTokenCommand
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(GetRequestTokenCommand));

        private readonly HttpClient _httpClient = new HttpClient();

        public GetRequestTokenCommand( RequestTokenParams paramz )   : base(paramz) {}

        public Creds GetToken()
        {
            var msg      = this.CreateMessage();
            SignMessage(msg);
            var response = _httpClient.SendAsync(msg).Result;
            var token    = base.ExtractToken(response);

            return token;
        }

        private HttpRequestMessage CreateMessage()
        {
            var msg = new HttpRequestMessage(HttpMethod.Get, OAuth.V1.Routes.RequestToken);
            var mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            return msg;
        }


        
    }
}