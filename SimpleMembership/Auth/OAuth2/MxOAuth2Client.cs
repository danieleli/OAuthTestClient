using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using DotNetOpenAuth.AspNet.Clients;
using System;
using DotNetOpenAuth.OAuth2;
using DotNetOpenAuth.Messaging;

namespace SimpleMembership.Auth.OAuth2
{
    public class MxOAuth2Client : OAuth2Client
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        private static readonly AuthorizationServerDescription MxDescription = new AuthorizationServerDescription
        {
            TokenEndpoint = new Uri("https://test.api.mxmerchant.com/v1/oauth/2/access_token"),
            AuthorizationEndpoint = new Uri("https://test.api.mxmerchant.com/v1/oauth/2/authorize"),
        };

        public MxOAuth2Client(string consumerKey, string consumerSecret)
            : base("MxOAuth2Client")
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            var builder = new UriBuilder(MxDescription.AuthorizationEndpoint);
            builder.AppendQueryArgs(
                new Dictionary<string, string> {
					{ "client_id", this._consumerKey },
					{ "redirect_uri", returnUrl.AbsoluteUri },
					{ "scope", "email" },
				});
            return builder.Uri;
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            throw new NotImplementedException();
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            // Note: Facebook doesn't like us to url-encode the redirect_uri value
            var builder = new UriBuilder(MxDescription.TokenEndpoint);
            builder.AppendQueryArgs(
                new Dictionary<string, string> {
					{ "client_id", this._consumerKey },
					{ "redirect_uri", NormalizeHexEncoding(returnUrl.AbsoluteUri) },
					{ "client_secret", this._consumerSecret },
					{ "code", authorizationCode },
					{ "scope", "email" },
				});

            using (WebClient client = new WebClient())
            {
                string data = client.DownloadString(builder.Uri);
                if (string.IsNullOrEmpty(data))
                {
                    return null;
                }

                var parsedQueryString = HttpUtility.ParseQueryString(data);
                return parsedQueryString["access_token"];
            }
        }

        /// <summary>
        /// Converts any % encoded values in the URL to uppercase.
        /// </summary>
        /// <param name="url">The URL string to normalize</param>
        /// <returns>The normalized url</returns>
        /// <example>NormalizeHexEncoding("Login.aspx?ReturnUrl=%2fAccount%2fManage.aspx") returns "Login.aspx?ReturnUrl=%2FAccount%2FManage.aspx"</example>
        /// <remarks>
        /// There is an issue in Facebook whereby it will rejects the redirect_uri value if
        /// the url contains lowercase % encoded values.
        /// </remarks>
        private static string NormalizeHexEncoding(string url)
        {
            var chars = url.ToCharArray();
            for (int i = 0; i < chars.Length - 2; i++)
            {
                if (chars[i] == '%')
                {
                    chars[i + 1] = char.ToUpperInvariant(chars[i + 1]);
                    chars[i + 2] = char.ToUpperInvariant(chars[i + 2]);
                    i += 2;
                }
            }
            return new string(chars);
        }
    }

    public static class ValidationHelper
    {
        /// <summary>
        /// Verifies something about the argument supplied to a method.
        /// </summary>
        /// <param name="condition">The condition that must evaluate to true to avoid an exception.</param>
        /// <param name="message">The message to use in the exception if the condition is false.</param>
        /// <param name="args">The string formatting arguments, if any.</param>
        /// <exception cref="ArgumentException">Thrown if <paramref name="condition"/> evaluates to <c>false</c>.</exception>
        public static void VerifyArgument(bool condition, string message, params object[] args)
        {
            //    Requires.NotNull(args, "args");
            // Assumes.True(message != null);
            if (!condition)
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, message, args));
            }
        }
    }

    public static class Extensions
    {
        internal static void AppendQueryArgs(this UriBuilder builder, IEnumerable<KeyValuePair<string, string>> args)
        {
            //Requires.NotNull(builder, "builder");

            if (args != null && args.Count() > 0)
            {
                StringBuilder sb = new StringBuilder(50 + (args.Count() * 10));
                if (!string.IsNullOrEmpty(builder.Query))
                {
                    sb.Append(builder.Query.Substring(1));
                    sb.Append('&');
                }
                sb.Append(CreateQueryString(args));

                builder.Query = sb.ToString();
            }
        }

        internal static string CreateQueryString(IEnumerable<KeyValuePair<string, string>> args)
        {
            //Requires.NotNull(args, "args");

            if (!args.Any())
            {
                return string.Empty;
            }
            StringBuilder sb = new StringBuilder(args.Count() * 10);

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

        private static readonly string[] UriRfc3986CharsToEscape = new[] { "!", "*", "'", "(", ")" };
    }
}
