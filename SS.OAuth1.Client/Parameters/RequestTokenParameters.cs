using System.Net.Http;

namespace SS.OAuth1.Client.Parameters
{
    public class RequestTokenParameters : OAuthParametersBase
    {
        public RequestTokenParameters(Creds consumer, string callback = "oob")
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.REQUEST_TOKEN)
        {
            Callback = callback;
        }

        public string Callback { get; private set; }
    }
}