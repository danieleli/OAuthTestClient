using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Web;
using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using System;
using DotNetOpenAuth.OAuth2;
using DotNetOpenAuth.Messaging;
using SimpleMembership.Auth.Extensions;

namespace SimpleMembership.Auth.OAuth2
{
    public static class Requester
    {
        public delegate void RedirectMethod(string url, bool endResponse);

        public static void RequestAuthentication(string authEndpoint, Uri returnUrl, string consumerKey, RedirectMethod redirectMethod)
        {    
            var builder = new UriBuilder(authEndpoint);

            builder.AppendQueryArgs(
                new Dictionary<string, string> {
					{ "client_id", consumerKey },
					{ "redirect_uri", returnUrl.AbsoluteUri },
					{ "scope", "email" },
				});

            redirectMethod(builder.Uri.AbsoluteUri, true);
        }
    }

    public static class Verifier
    {
        public static AuthenticationResult VerifyAuthentication(string tokenEndpoint, string consumerKey, string consumerSecret, NameValueCollection queryString, Uri returnPageUrl)
        {
            IDictionary<string, string> userData;
            string accessToken;
            try
            {
                var code = queryString["code"];
                accessToken = QueryAccessToken(tokenEndpoint, consumerKey, consumerSecret, returnPageUrl, code);
                userData = GetUserData(accessToken);

                if(userData==null) throw new Exception();
            }
            catch (Exception)
            {
               return AuthenticationResult.Failed;
            }
            
 
            string id = userData["id"];
            string name;

            // Some oAuth providers do not return value for the 'username' attribute. 
            // In that case, try the 'name' attribute. If it's still unavailable, fall back to 'id'
            if (!userData.TryGetValue("username", out name) && !userData.TryGetValue("name", out name))
            {
                name = id;
            }

            // add the access token to the user data dictionary just in case page developers want to use it
            userData["accesstoken"] = accessToken;

            return new AuthenticationResult(
                isSuccessful: true, provider: "MxOAuth2Client", providerUserId: id, userName: name, extraData: userData);
        }

        private static string QueryAccessToken(string tokenEndpoint, string consumerKey, string consumerSecret, Uri returnUrl, string authorizationCode)
        {
            // Note: Facebook doesn't like us to url-encode the redirect_uri value
            var builder = new UriBuilder(tokenEndpoint);
            builder.AppendQueryArgs(
                new Dictionary<string, string> {
					{ "client_id", consumerKey },
					{ "redirect_uri", returnUrl.AbsoluteUri.NormalizeHexEncoding() },
					{ "client_secret", consumerSecret },
					{ "code", authorizationCode },
					{ "scope", "email" },
				});

            using (var client = new WebClient())
            {
                var data = client.DownloadString(builder.Uri);
                if (string.IsNullOrEmpty(data))
                {
                    return null;
                }

                var parsedQueryString = HttpUtility.ParseQueryString(data);
                return parsedQueryString["access_token"];
            }
        }

        public static IDictionary<string, string> GetUserData(string accessToken)
        {
            throw new NotImplementedException();
            return new Dictionary<string, string>();
        }

    }

    public class MxOAuth2Client : OAuth2Client
    {
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        //public const string AUTHORIZATION_ENDPOINT = "https://test.api.mxmerchant.com/v1/oauth/2/authorize";
        //public const string TOKEN_ENDPOINT = "https://test.api.mxmerchant.com/v1/oauth/2/access_token";

        public const string AUTHORIZATION_ENDPOINT = "http://localhost:50172/oauth/authorize";
        public const string TOKEN_ENDPOINT = "http://localhost:50172/oauth/access_token";

        public MxOAuth2Client(string consumerKey, string consumerSecret)
            : base("MxOAuth2Client")
        {
            _consumerKey = consumerKey;
            _consumerSecret = consumerSecret;
        }


        public override void RequestAuthentication(HttpContextBase context, Uri returnUrl)
        {

            Requester.RequestAuthentication(AUTHORIZATION_ENDPOINT, returnUrl, _consumerKey, context.Response.Redirect);
        }


        public override AuthenticationResult VerifyAuthentication(HttpContextBase context, Uri returnPageUrl)
        {
            return Verifier.VerifyAuthentication(TOKEN_ENDPOINT, _consumerKey, _consumerSecret, context.Request.QueryString,
                                          returnPageUrl);
        }

        protected override Uri GetServiceLoginUrl(Uri returnUrl)
        {
            throw new NotImplementedException();
            //return Requester.GetServiceLoginUrl(AUTHORIZATION_ENDPOINT, _consumerKey, returnUrl);
        }

        protected override IDictionary<string, string> GetUserData(string accessToken)
        {
            throw new NotImplementedException();
        }

        protected override string QueryAccessToken(Uri returnUrl, string authorizationCode)
        {
            throw new NotImplementedException();
        }

    }
}
