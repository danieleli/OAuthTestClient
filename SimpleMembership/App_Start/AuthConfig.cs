using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleMembership.Auth;
using SimpleMembership.Auth.OAuth1a;


namespace SimpleMembership
{
    public static class AuthConfig
    {
        public static void RegisterAuth()
        {
            // To let users of this site log in using their accounts from other sites such as Microsoft, Facebook, and Twitter,
            // you must update this site. For more information visit http://go.microsoft.com/fwlink/?LinkID=252166

            //OAuthWebSecurity.RegisterMicrosoftClient(
            //    clientId: "",
            //    clientSecret: "");

            //OAuthWebSecurity.RegisterTwitterClient(
            //    consumerKey: Credentials.Twitter.KEY,
            //    consumerSecret: Credentials.Twitter.SECRET);

            //OAuthWebSecurity.RegisterFacebookClient(
            //    appId: Credentials.Facebook.KEY,
            //    appSecret: Credentials.Facebook.SECRET);

            //OAuthWebSecurity.RegisterGoogleClient();

            //OAuthWebSecurity.RegisterClient(
            //    new MxClient(Credentials.MxMerchant.KEY, Credentials.MxMerchant.SECRET), "MxMerchant", null);
        }
    }
}
