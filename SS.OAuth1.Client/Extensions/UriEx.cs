using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.OAuth1.Client.Extensions
{
    public static class UriEx
    {
        public static string GetBaseStringUri(this Uri uri)
        {
            var scheme = uri.Scheme;
            var host = uri.Host;

            var rtn = string.Format("{0}://{1}", scheme, host);
            return rtn;
        }
    }

}
