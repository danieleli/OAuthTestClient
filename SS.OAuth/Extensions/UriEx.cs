using System;

namespace SS.OAuth.Extensions
{
    public static class UriEx
    {
        public static string GetBaseStringUri(this Uri uri)
        {
            var scheme = uri.Scheme;
            var host = uri.Host;
            var path = uri.AbsolutePath;

            var rtn = string.Format("{0}://{1}{2}", scheme, host, path);
            return rtn;
        }
    }

}
