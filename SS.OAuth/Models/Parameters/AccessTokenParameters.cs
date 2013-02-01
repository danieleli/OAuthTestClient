#region

using System.Collections.Specialized;

#endregion

namespace SS.OAuth.Models.Parameters
{
    public class AccessTokenParameters : BaseParameters
    {
        
        public Creds RequestToken { get; set; }
        public string Verifier { get; set; }
        public string SessionHandle { get; set; }

        public AccessTokenParameters(Creds consumer, Creds requestToken, string verifier)
            : base(consumer)
        {
            RequestToken = requestToken;
            Verifier = verifier;
        }

        public override NameValueCollection GetOAuthParams()
        {
            var paramPairs = base.GetOAuthParams();
            paramPairs.AddIfNotNullOrEmpty(OAuth.V1.Keys.TOKEN, this.RequestToken.Key);
            paramPairs.AddIfNotNullOrEmpty(OAuth.V1.Keys.VERIFIER, this.Verifier);

            return paramPairs;
        }
    }
}