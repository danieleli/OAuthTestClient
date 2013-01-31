#region

using System;

#endregion

namespace SS.OAuth1.Client.Helpers
{
    public static class OAuth
    {
        private static readonly Random random = new Random();

        public static string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }

        public static string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return random.Next(123400, 9999999).ToString();
        }

        public static class V1
        {
            public const string AUTHORIZATION_HEADER = "Authorization";


            public static class Routes
            {
                public const string BASE_ROUTE = G.BASE_API_URL + "/OAuth/1A";

                public const string REQUEST_TOKEN = BASE_ROUTE + "/RequestToken";
                public const string ACCESS_TOKEN = BASE_ROUTE + "/AccessToken";
                private const string AUTHORIZE_TOKEN = BASE_ROUTE + "/AuthorizeToken?token={0}&isAuthorized=true";

                public static string GetAuthorizeTokenRoute(string token)
                {
                    var url = String.Format(AUTHORIZE_TOKEN, token);
                    return url;
                }

                public static class WebViews
                {
                    private const string AUTHORIZE = G.BASE_SITE_URL + "/oauth/authorize?oauth_token={0}";

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