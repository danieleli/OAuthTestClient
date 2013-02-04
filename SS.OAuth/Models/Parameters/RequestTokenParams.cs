
using System.Collections.Specialized;
using SS.OAuth.Extensions;

namespace SS.OAuth.Models.Parameters
{
    public class RequestTokenParams : BaseParams
    {
        public string Callback { get; private set; }

        public RequestTokenParams( Creds consumer, string callback = "oob", bool includeVersion = true, string realm = null )
            : base(includeVersion, realm)
        {
            Consumer = consumer;
            Callback = callback;
        }
        public override NameValueCollection ToCollection()
        {
            var col = base.ToCollectionInternal();
            col.AddIfNotNullOrEmpty(OAuth.V1.Keys.CALLBACK, this.Callback);
            return col;
        }
    }
}