using System.Collections.Specialized;
using SS.OAuth.Extensions;

namespace SS.OAuth.Models.Parameters
{
    public class VerifierTokenParams : BaseParams
    {
        public string UserToken { get; set; }

        public VerifierTokenParams(Creds consumer, Creds requestToken, string userToken)
        {
            Consumer = consumer;
            RequestToken = requestToken;
            UserToken = userToken;
        }


        public override NameValueCollection ToCollection()
        {
            var col = base.ToCollectionInternal();
            col.AddIfNotNullOrEmpty(OAuth.V1.Keys.CALLBACK, this.UserToken);
            return col;
        }
    }
}