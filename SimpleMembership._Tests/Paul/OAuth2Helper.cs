using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Script.Serialization;

namespace MXM.API.Test.Controllers
{
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

            msg.Content =  new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    {"code", code},
                    {"client_id", clientCreds.Key},
                    {"client_secret", clientCreds.Secret},
                    {"redirect_uri", ""},
                    {"grant_type", "authorization_code"}
                });
            var response = MsgHelper.Send(msg);
            var result = response.Content.ReadAsStringAsync().Result;
            var rtn = new JavaScriptSerializer().DeserializeObject(result);
            return rtn;
        }

        public static object RefreshToken(string refreshToken, Creds clientCreds, string redirectUri)
        {
            var msg = MsgHelper.CreateRequestMessage(OAuthRoutes.V2.REFRESH_TOKEN, HttpMethod.Post);

            msg.Content =  new FormUrlEncodedContent(new Dictionary<string, string>()
                {
                    {"refresh_token", refreshToken},
                    {"client_id", clientCreds.Key},
                    {"client_secret", clientCreds.Secret},
                    {"redirect_uri", redirectUri},
                    {"grant_type", "refresh_token"}
                });

            var response = MsgHelper.Send(msg);
            var rtn = GetJsonObject(response);

            return rtn;
        }

        private static object GetJsonObject(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsStringAsync().Result;
            var rtn = new JavaScriptSerializer().DeserializeObject(result);
            return rtn;
        }
    }
}