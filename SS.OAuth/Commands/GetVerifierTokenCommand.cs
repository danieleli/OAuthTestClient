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

        protected GetVerifierTokenCommand( VerifierTokenParams p )
            : base(p)
        {
            UserToken = p.UserToken;
        }

        public Creds GetToken()
        {
            var msg      = this.CreateMessage();
            SignMessage(msg);
            var response = base.HttpClient.SendAsync(msg).Result;
            var token    = base.ExtractToken(response);

            return token;
        }

        private HttpRequestMessage CreateMessage()
        {
            var msg = new HttpRequestMessage(HttpMethod.Post, OAuth.V1.Routes.GetAuthorizeTokenRoute(this.UserToken));
            var mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            msg.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));

            return msg;
        }
    }
}