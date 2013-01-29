using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SS.OAuth1.Client.Helpers;
using SS.OAuth1.Client.Models;

namespace SS.OAuth1.Client._Tests.Tests
{
    [TestFixture]
    public class OAuth2Test
    {
        //private readonly Creds _user = G.TestCreds.DanUser;
        //private readonly Creds _consumer = G.TestCreds.DanConsumer;

        //[Test]
        //public void OAuth_2()
        //{

        //    var AC = GetAuthorizationCode(authorizer_AT, base.consumerKey, base.consumerSecret);
        //    Assert.IsNotNull(AC);

        //    var AT = GetOAuth2AccessToken(AC);
        //    Assert.IsNotNull(AT);

        //    var AT_bad = GetOAuth2AccessToken(AC);
        //    Assert.IsNull(AT_bad);

        //    var AT2 = GetOAuth2AccessToken((string)(AT as Dictionary<string, object>)["refresh_token"]);
        //    Assert.IsNotNull(AT2);

        //    var AT3 = GetOAuth2AccessToken((string)(AT as Dictionary<string, object>)["refresh_token"]);
        //    Assert.IsNotNull(AT3);
        //    //var at = GetAccessToken(rt1, ov);

        //    //Assert.IsNotNull(at);

        //}


        //public string GetAuthorizationCode(NameValueCollection rt, string client_id = null, string cs = null)
        //{
        //    string requestURL = baseAddress + "/OAuth/2/AuthorizationCode?client_id=" + (client_id ?? consumerKey) + "&state=" + Guid.NewGuid().ToString() + "&redirect_uri=https%3A%2F%2Fapi.pps.io%2Foauth%2F2%2Fcallback&response_type=code";

        //    HttpRequestMessage authRequest = new HttpRequestMessage
        //    {
        //        RequestUri = new Uri(requestURL),
        //        Method = HttpMethod.Post,
        //    };

        //    string mediaType = FormUrlEncodedMediaTypeFormatter.DefaultMediaType.MediaType;
        //    authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType));
        //    authRequest.Sign(SignatureMethod.OAuth1A, consumerKey, consumerSecret, rt["oauth_token"], rt["oauth_token_secret"], null, null, null, null);

        //    HttpClient httpClient = new HttpClient();
        //    HttpResponseMessage authResponse = httpClient.SendAsync(authRequest).Result;

        //    return System.Web.HttpUtility.ParseQueryString(authResponse.Headers.Location.Query)["code"];
        //}


        //public object GetOAuth2AccessToken(string code, string client_id = null, string client_secret = null)
        //{
        //    string requestURL = baseAddress + "/OAuth/2/AccessToken";

        //    HttpRequestMessage authRequest = new HttpRequestMessage
        //    {
        //        RequestUri = new Uri(requestURL),
        //        Method = HttpMethod.Post,
        //    };

        //    client_id = client_id ?? consumerKey;
        //    client_secret = client_secret ?? consumerSecret;

        //    authRequest.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(JsonMediaTypeFormatter.DefaultMediaType.MediaType));

        //    if (code.StartsWith("C"))
        //        authRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "code", code }, { "client_id", client_id }, { "client_secret", client_secret }, { "redirect_uri", "https://api.pps.io/oauth/2/callback" }, { "grant_type", "authorization_code" } });
        //    else
        //        authRequest.Content = new FormUrlEncodedContent(new Dictionary<string, string>() { { "refresh_token", code }, { "client_id", client_id }, { "client_secret", client_secret }, { "redirect_uri", "https://api.pps.io/oauth/2/callback" }, { "grant_type", "refresh_token" } });

        //    HttpClient httpClient = new HttpClient();
        //    HttpResponseMessage authResponse = httpClient.SendAsync(authRequest).Result;

        //    var result = new JavaScriptSerializer().DeserializeObject(authResponse.Content.ReadAsStringAsync().Result);

        //    return result;
        //}
    }
}
