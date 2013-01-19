#region

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Script.Serialization;

#endregion

namespace MXM.API.Test.Controllers
{
    public static partial class OAuthRoutes
    {
        public static class V2
        {
            public const string ROUTE = G.BASE_URL + "/OAuth/2/";

            public const string ACCESS_TOKEN = ROUTE + "AccessToken";

            public const string AUTHORIZATION_CODE =
                ROUTE + "AuthorizationCode?client_id={0}&state={1}&redirect_uri={2}";

            public const string REFRESH_TOKEN = ROUTE + "TBD";
        }
    }

    public enum GrantType
    {
        AUTHORIZATION_CODE,
        REFRESH_TOKEN
    }

    public static class OAuth2Helper
    {
        public static HttpResponseMessage GetAuthorizationCode(Creds consumer, string returnUri)
        {
            var state = Guid.NewGuid().ToString();
            var url = string.Format(OAuthRoutes.V2.AUTHORIZATION_CODE, consumer.Key, state, returnUri);

            var msg = MsgHelper.CreateRequestMessage(url, HttpMethod.Post);
            var result = MsgHelper.Send(msg);

            return result;
        }

        public static object GetAccessToken(string code, Creds clientCreds)
        {
            var msg = MsgHelper.CreateRequestMessage(OAuthRoutes.V2.ACCESS_TOKEN, HttpMethod.Post);

            var contentDic = GetContentDictionary(clientCreds, "", GrantType.AUTHORIZATION_CODE);
            contentDic.Add("code", code);

            msg.Content = new FormUrlEncodedContent(contentDic);

            var response = MsgHelper.Send(msg);
            var result = response.Content.ReadAsStringAsync().Result;

            var rtn = new JavaScriptSerializer().DeserializeObject(result);

            return rtn;
        }

        public static object RefreshToken(string refreshToken, Creds clientCreds, string redirectUri)
        {
            var msg = MsgHelper.CreateRequestMessage(OAuthRoutes.V2.REFRESH_TOKEN, HttpMethod.Post);

            var contentDic = GetContentDictionary(clientCreds, redirectUri, GrantType.REFRESH_TOKEN);
            contentDic.Add("refresh_token", refreshToken);

            msg.Content = new FormUrlEncodedContent(contentDic);

            var response = MsgHelper.Send(msg);
            var rtn = GetJsonObject(response);

            return rtn;
        }

        private static Dictionary<string, string> GetContentDictionary(Creds clientCreds, string redirectUri,
                                                                       GrantType grantType)
        {
            return new Dictionary<string, string>
                {
                    {"client_id", clientCreds.Key},
                    {"client_secret", clientCreds.Secret},
                    {"redirect_uri", redirectUri},
                    {"grant_type", grantType.ToString().ToLower()}
                };
        }

        private static object GetJsonObject(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsStringAsync().Result;
            var rtn = new JavaScriptSerializer().DeserializeObject(result);
            return rtn;
        }
    }
}