using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Commands
{
    public class GetVerifierTokenCommand : BaseTokenCommand
    {
        private string UserToken { get; set; }

        public GetVerifierTokenCommand( VerifierTokenParams p ) : base(p)
        {
            UserToken = p.UserToken;
        }

        public Creds GetToken()
        {
            var msg      = this.CreateMessage();
            SignMessage(msg);
            var response = base.HttpClient.SendAsync(msg).Result;
            var token    = this.ExtractToken(response);

            return token;
        }

        private HttpRequestMessage CreateMessage()
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, OAuth.V1.Routes.GetAuthorizeTokenRoute(this.UserToken));
            var mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            return msg;
        }
        
        protected override Creds ExtractToken( HttpResponseMessage response )
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                base.ThrowException(response);
            }

            var content = response.Content.ReadAsFormDataAsync().Result;

            if (content == null) throw new ArgumentException("No content found.");

            var key = content[OAuth.V1.Keys.TOKEN];
            var secret = content[OAuth.V1.Keys.VERIFIER];
            var token = new Creds(key, secret);

            return token;
        }
    }
}