using System;
using System.Collections.Generic;
using System.Text;

namespace SS.OAuth1.Client.Parameters
{
    public static class Extensions
    {
        public static string Stringify(this SortedDictionary<string, string> dic)
        {
            var sb = new StringBuilder();
            var isFirstItem = true;

            foreach (var entry in dic)
            {
                if (!isFirstItem)
                {
                    sb.Append(",");
                }
                var key = Uri.EscapeUriString(entry.Key);
                var value = Uri.EscapeUriString(entry.Value);
                sb.Append(string.Format("{0}=\"{1}\"", key, value));
                isFirstItem = false;
            }

            return sb.ToString();
        }

        public static void AddIfNotNullOrEmpty(this IDictionary<string, string> dic, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                dic.Add(key, value);
            }
        }
    }
}