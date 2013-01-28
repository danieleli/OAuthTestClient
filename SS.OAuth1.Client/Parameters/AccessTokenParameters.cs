#region

using System.Net.Http;

#endregion

namespace SS.OAuth1.Client.Parameters
{
    public class AccessTokenParameters : OAuthParametersBase
    {
        public AccessTokenParameters(Creds consumer, Creds token)
            : base(consumer, HttpMethod.Post, OAuth.V1.Routes.ACCESS_TOKEN)
        {
            NullCheck(token, "token");
            NullCheck(token.Key, "requestToken");
            NullCheck(token.Secret, "requestTokenSecret");

            Token = token;
        }

        public Creds Token { get; set; }
        public string RequestTokenSecret { get; set; }
        public string SessionHandle { get; set; }
        public override string GetAuthHeader()
        {
            throw new System.NotImplementedException();
        }
    }
}