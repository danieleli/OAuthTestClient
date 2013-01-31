using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using NUnit.Framework;
using PPS.API.Common.Helpers;
using PPS.API.Common.Extensions;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using SS.OAuth1.Client.Helpers;

namespace MXM.API.Test.Controllers
{
    [TestClass]
    public class OauthControllerTest : ControllerTestBase
    {

        protected new string consumerKey = "6rexu7a9ezawyss6krucyh9v"; //42798D00-8402-4F19-9956-044E8573C9E5";
        protected new string consumerSecret = "oe793wU6pxvpSAUm7U1nF8Ol000="; //CxCNVJMtU4Uy+XO0y0TDmpFA1A4=";

        protected string userkey = "dan+test"; //42798D00-8402-4F19-9956-044E8573C9E5";
        protected string usersecret = "cc2c0ef703029feddef7be3736aa8b53fc295ae7"; //CxCNVJMtU4Uy+XO0y0TDmpFA1A4=";




        [TestMethod]
        public void OAuth_2()
        {
            var authorizer_RT = GetRequstToken();
            Assert.IsNotNull(authorizer_RT);
            var authorizer_AT = GetAccessToken(authorizer_RT, null);
            Assert.IsNotNull(authorizer_AT);



            var AC = GetAuthorizationCode(authorizer_AT, base.consumerKey, base.consumerSecret);
            Assert.IsNotNull(AC);

            var AT = GetOAuth2AccessToken(AC);
            Assert.IsNotNull(AT);

            var AT_bad = GetOAuth2AccessToken(AC);
            Assert.IsNull(AT_bad);

            var AT2 = GetOAuth2AccessToken((string)(AT as Dictionary<string, object>)["refresh_token"]);
            Assert.IsNotNull(AT2);

            var AT3 = GetOAuth2AccessToken((string)(AT as Dictionary<string, object>)["refresh_token"]);
            Assert.IsNotNull(AT3);
            //var at = GetAccessToken(rt1, ov);

            //Assert.IsNotNull(at);

        }

        public string GetAuthorizationCode(NameValueCollection rt, string client_id = null, string cs = null)
        {
            string requestURL = baseAddress + "/OAuth/2/AuthorizationCode?client_id=" + (client_id ?? consumerKey) + "&state=" + Guid.NewGuid().ToString() + "&redirect_uri=https%3A%2F%2Fapi.pps.io%2Foauth%2F2%2Fcallback&response_type=code";

            HttpRequestMessage authRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(requestURL),
                Method = HttpMethod.Post,
            };

            string mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            authRequest.Sign(SignatureMethod.OAuth1A, consumerKey, consumerSecret, rt["oauth_token"], rt["oauth_token_secret"], null, null, null, null);

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage authResponse = httpClient.SendAsync(authRequest).Result;

            return System.Web.HttpUtility.ParseQueryString(authResponse.Headers.Location.Query)["code"];
        }


        public object GetOAuth2AccessToken(string code, string client_id = null, string client_secret = null)
        {
            string requestURL = baseAddress + "/OAuth/2/AccessToken";

            HttpRequestMessage authRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(requestURL),
                Method = HttpMethod.Post,
            };

            client_id = client_id ?? consumerKey;
            client_secret = client_secret ?? consumerSecret;

            authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaTypeFormatter.DefaultMediaType.MediaType));

            if (code.StartsWith("C"))
                authRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "code", code }, { "client_id", client_id }, { "client_secret", client_secret }, { "redirect_uri", "https://api.pps.io/oauth/2/callback" }, { "grant_type", "authorization_code" } });
            else
                authRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "refresh_token", code }, { "client_id", client_id }, { "client_secret", client_secret }, { "redirect_uri", "https://api.pps.io/oauth/2/callback" }, { "grant_type", "refresh_token" } });

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage authResponse = httpClient.SendAsync(authRequest).Result;

            var result = new JavaScriptSerializer().DeserializeObject(authResponse.Content.ReadAsStringAsync().Result);

            return result;
        }



        public string GetTokenVerifier(NameValueCollection rt, string token, string consumerKey = null, string consumerSecret = null)
        {
            //48200
            //100001320
            //string requestURL = baseAddress + "/OAuth/1A/AuthorizeToken?token=" + (token ?? rt["oauth_token"]) + "&isAuthorized=true";
            string requestURL = "https://test.api.mxmerchant.com/v1/OAuth/1A/AuthorizeToken?token=" + (userToken) + "&isAuthorized=true";
            authRequest.Sign(consumerKey, consumerSecret, requestTokenKey, requestTokenSecret, null, null, null);
            
            HttpRequestMessage authRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(requestURL),
                Method = HttpMethod.Post
            };

            string mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            
            authRequest.Sign(consumerKey, consumerSecret, requestTokenKey, requestTokenSecret, null, null, null);

            authRequest.Sign(SignatureMethod.OAuth1A, consumerKey ?? base.consumerKey, consumerSecret ?? base.consumerSecret, rt["oauth_token"], rt["oauth_token_secret"], null, null, null);
            authRequest.Sign(consumerKey, consumerSecret, requestTokenKey, requestTokenSecret, null, null, null);
            HttpClient httpClient = new HttpClient();
            var result = httpClient.SendAsync(authRequest);

            HttpResponseMessage authResponse = result.Result;

            if (authResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return null;
            if (authResponse.Headers.Location == null)
                return authResponse.Content.ReadAsFormDataAsync().Result["oauth_verifier"];
            else
                return authResponse.Headers.Location.ParseQueryString()["oauth_verifier"];
        }


        public NameValueCollection GetRequstToken(string ck = null, string cs = null)
        {
            //string requestURL = baseAddress + "/OAuth/1A/RequestToken";
            string requestURL = "https://test.api.mxmerchant.com/v1/OAuth/1A/RequestToken";

            HttpRequestMessage authRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(requestURL),
                Method = HttpMethod.Post
            };

            string mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            authRequest.Sign(SignatureMethod.OAuth1A, ck ?? consumerKey, cs ?? consumerSecret, null, null, null, null, "oob", null);

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage authResponse = httpClient.SendAsync(authRequest).Result;

            return authResponse.Content.ReadAsFormDataAsync().Result;
        }

        public NameValueCollection GetAccessToken(NameValueCollection rt, string verifier, string ck = null, string cs = null)
        {
            //string requestURL = baseAddress + "/OAuth/1A/AccessToken";
            string requestURL = "https://test.api.mxmerchant.com/v1/OAuth/1A/AccessToken";

            HttpRequestMessage authRequest = new HttpRequestMessage
            {
                RequestUri = new Uri(requestURL),
                Method = HttpMethod.Post
            };

            string mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
            authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
            authRequest.Sign(SignatureMethod.OAuth1A, ck ?? consumerKey, cs ?? consumerSecret, rt["oauth_token"], rt["oauth_token_secret"], null, null, null, verifier);

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage authResponse = httpClient.SendAsync(authRequest).Result;

            if (authResponse.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                return null;

            return authResponse.Content.ReadAsFormDataAsync().Result;
        }
    }
}
