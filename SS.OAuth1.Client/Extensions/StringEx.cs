using System.Text;

namespace SS.OAuth1.Client.Extensions
{
    public static class StringEx
    {
        public const string UNRESERVED_CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_.~";

        public static string UrlEncodeForOAuth(this string toEncode)
        {
            var sb = new StringBuilder();
            foreach (char symbol in toEncode)
            {
                if (UNRESERVED_CHARS.IndexOf(symbol) != -1)
                {
                    sb.Append(symbol);
                }
                else
                {
                    sb.AppendFormat("%{0:X2}", (int)symbol);
                }
            }
            return sb.ToString();
        }
    }
}