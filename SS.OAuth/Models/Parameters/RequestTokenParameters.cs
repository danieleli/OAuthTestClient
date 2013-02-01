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
    }
}