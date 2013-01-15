using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleMembership.Auth.OAuth2;

namespace SimpleMembership.Auth.Extensions
{
    public static class UriBuilderExtensions
    {
        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };

        public static void AppendQueryArgs(this UriBuilder builder, IDictionary<string, string> args)
        {
            //Requires.NotNull(builder, "builder");

            if (args != null && args.Count() > 0)
            {
                var sb = new StringBuilder(50 + (args.Count() * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    sb.Append(builder.Query.Substring(1));
                    sb.Append('&');
                }
                sb.Append(CreateQueryString(args));

                builder.Query = sb.ToString();
            }
        }

        internal static string CreateQueryString(IDictionary<string, string> args)
        {
            //Requires.NotNull(args, "args");

            if (!args.Any())
            {
                return string.Empty;
            }
            var sb = new StringBuilder(args.Count() * 10);

            foreach (var p in args)
            {
                ValidationHelper.VerifyArgument(!string.IsNullOrEmpty(p.Key), "Unexpected Null Or Empty Key");
                ValidationHelper.VerifyArgument(p.Value != null, "Unexpected Null Value", p.Key);
                sb.Append(EscapeUriDataStringRfc3986(p.Key));
                sb.Append('=');
                sb.Append(EscapeUriDataStringRfc3986(p.Value));
                sb.Append('&');
            }
            sb.Length--; // remove trailing &

            return sb.ToString();
        }

        internal static string EscapeUriDataStringRfc3986(string value)
        {
            //Requires.NotNull(value, "value");

            // Start with RFC 2396 escaping by calling the .NET method to do the work.
            // This MAY sometimes exhibit RFC 3986 behavior (according to the documentation).
            // If it does, the escaping we do that follows it will be a no-op since the
            // characters we search for to replace can't possibly exist in the string.
            StringBuilder escaped = new StringBuilder(Uri.EscapeDataString(value));

            // Upgrade the escaping to RFC 3986, if necessary.
            for (int i = 0; i < UriRfc3986CharsToEscape.Length; i++)
            {
                escaped.Replace(UriRfc3986CharsToEscape[i], Uri.HexEscape(UriRfc3986CharsToEscape[i][0]));
            }

            // Return the fully-RFC3986-escaped string.
            return escaped.ToString();
        }

    }
}