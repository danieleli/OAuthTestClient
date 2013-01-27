#region

using System;
using System.Collections.Generic;

#endregion

namespace SimpleMembership._Tests.Paul.OAuth1
{


    public static class OAuth
    {
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

                public static string GetAuthorizeTokenWebView(string token)
                {
                    var url = String.Format(AUTHORIZE_TOKEN, token);
                    url = url.Replace("https", "http");
                    url = url.Replace("/v1", "");

                    return url;
                }
            }
        }
    }
}