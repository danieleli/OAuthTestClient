using System;
using System.Net;
using System.Net.Http;
using System.Web;
using SS.OAuth.Factories;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace SS.OAuth.Commands
{
    public class BaseTokenCommand
    {
        private HttpClient _httpClient;

        protected HttpClient HttpClient
        {
            get { return _httpClient ?? (_httpClient = new HttpClient()); }
            set { _httpClient = value; }
        }

        protected BaseTokenCommand(BaseParams p)
        {
            HeaderFactory = new OAuthHeaderFactory(p);
        }

        private OAuthHeaderFactory HeaderFactory { get; set; }

        protected void SignMessage( HttpRequestMessage msg )
        {
            var sig = this.HeaderFactory.GetSignature(msg);
            var header = this.HeaderFactory.CreateHeader(sig);
            msg.Headers.Add(OAuth.V1.AUTHORIZATION_HEADER, header);
        }

        protected virtual Creds ExtractToken( HttpResponseMessage response )
        {
            if (response.StatusCode != HttpStatusCode.OK)
            {
                ThrowException(response);
            }

            var content = response.Content.ReadAsFormDataAsync().Result;

            if (content == null) throw new ArgumentException("No content found.");

            var key = content[OAuth.V1.Keys.TOKEN];
            var secret = content[OAuth.V1.Keys.TOKEN_SECRET];
            var token = new Creds(key, secret);

            return token;
        }

        protected void ThrowException(HttpResponseMessage response)
        {
            var content = response.Content.ReadAsStringAsync().Result;

            switch (response.StatusCode)
            {
                case HttpStatusCode.Unauthorized:
                    {
                        throw new UnauthorizedAccessException(content);
                    }
                default:
                    {
                        throw new HttpException(content);
                    }
            }

            


        }
    }
}
