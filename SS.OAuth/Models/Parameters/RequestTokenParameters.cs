using System.Collections.Specialized;

namespace SS.OAuth.Models.Parameters
{
    public class RequestTokenParameters : BaseParameters
    {
        public string Callback { get; private set; }

        public RequestTokenParameters(Creds consumer, string callback = "oob")
            : base(consumer)
        {
            Callback = callback;
        }

        public override NameValueCollection GetOAuthParams()
        {
            var paramPairs = base.GetOAuthParams();
            paramPairs.AddIfNotNullOrEmpty(OAuth.V1.Keys.CALLBACK, this.Callback);

            return paramPairs;
        }
    }
}