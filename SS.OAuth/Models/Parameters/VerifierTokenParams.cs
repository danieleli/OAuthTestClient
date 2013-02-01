namespace SS.OAuth.Models.Parameters
{
    public class VerifierTokenParams : BaseParams
    {
        public Creds RequestToken { get; set; }
        public string UserToken { get; set; }

        public VerifierTokenParams(Creds consumer, Creds requestToken, string userToken)
        {
            Consumer = consumer;
            RequestToken = requestToken;
            UserToken = userToken;
        }
    }
}