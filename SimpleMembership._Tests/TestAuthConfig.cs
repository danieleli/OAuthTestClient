using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Web.WebPages.OAuth;
using SimpleMembership.Auth.OAuth1a;

namespace SimpleMembership._Tests
{
    public static class TestAuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            OAuthWebSecurity.RegisterTwitterClient(
                consumerKey: Twitter.KEY,
                consumerSecret: Twitter.SECRET);

            OAuthWebSecurity.RegisterFacebookClient(
                appId: Facebook.KEY,
                appSecret: Facebook.SECRET);

            OAuthWebSecurity.RegisterGoogleClient();

            OAuthWebSecurity.RegisterClient(
                new MxClient(MxMerchant.KEY, MxMerchant.SECRET), "MxMerchant", null);
        }

        public static class Twitter
        {
            public static string KEY = "iOBfXhNIVVZ0M1J4MPC8gQ";
            public static string SECRET = "4RtOw7W8OzEuPOlLsV4kDR0WKn3miAtgSQLxS2GTFpE";
        }

        public static class Facebook
        {
            public static string KEY = "165121776882907";
            public static string SECRET = "7624e78350c663a2414cb6cfc7f82ce5";
        }
        public static class MxMerchant
        {
            public static string KEY = "6rexu7a9ezawyss6krucyh9v";
            public static string SECRET = "oe793wU6pxvpSAUm7U1nF8Ol000=";
        }
    }
}
