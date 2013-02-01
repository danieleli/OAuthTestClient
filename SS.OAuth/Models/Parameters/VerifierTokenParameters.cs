using System.Collections.Specialized;

namespace SS.OAuth.Models.Parameters
{
    public class VerifierTokenParameters : BaseParameters
    {
        public Creds RequestToken { get; set; }
        public string UserToken { get; set; }

        // Constructor
        public VerifierTokenParameters(Creds consumer, Creds requestToken, string userToken)
            : base(consumer)
        {
            RequestToken = requestToken;
            UserToken = userToken;
        }
    }
}