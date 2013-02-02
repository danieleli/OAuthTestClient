
namespace SS.OAuth.Models.Parameters
{
    public class RequestTokenParams : BaseParams
    {
        public string Callback { get; private set; }

        public RequestTokenParams(Creds consumer, string callback = "oob")
        {
            Consumer = consumer;
            Callback = callback;
        }
    }
}