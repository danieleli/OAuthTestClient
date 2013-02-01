using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SS.OAuth
{
    public static class OAuth
    {
        public static class V1
        {
            public const string AUTHORIZATION_HEADER = "Authorization";

            public static class Keys
            {
                public const string REALM = "realm";
                public const string CALLBACK = "oauth_callback";
                public const string CONSUMER_KEY = "oauth_consumer_key";
                public const string NONCE = "oauth_nonce";
                public const string SIGNATURE = "oauth_signature";
                public const string SIGNATURE_METHOD = "oauth_signature_method";
                public const string TIMESTAMP = "oauth_timestamp";
                public const string TOKEN = "oauth_token";
                public const string TOKEN_SECRET = "oauth_token_secret";
                public const string VERIFIER = "oauth_verifier";
                public const string VERSION = "oauth_version";
            }

            public static class Values
            {
                public const string VERSION = "1.0";
                public const string SIGNATURE_METHOD = "HMAC-SHA1";
            }

            public static class Routes
            {
                public static readonly string BaseRoute = G.BaseApiUrl + "/OAuth/1A";

                public static readonly string RequestToken = BaseRoute + "/RequestToken";
                public static readonly string AccessToken = BaseRoute + "/AccessToken";
                private static readonly string AuthorizeToken = BaseRoute + "/AuthorizeToken?token={0}&isAuthorized=true";

                public static string GetAuthorizeTokenRoute(string token)
                {
                    var url = String.Format(AuthorizeToken, token);
                    return url;
                }

                public static class WebViews
                {
                    private static readonly string AUTHORIZE = G.BaseSiteUrl + "/oauth/authorize?oauth_token={0}";

                    public static string GetAuthorizeRoute(string token)
                    {
                        var url = String.Format(AUTHORIZE, token);
                        return url;
                    }
                }

            }
        }
    }
}
