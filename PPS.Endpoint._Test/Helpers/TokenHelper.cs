using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SS.OAuth.Commands;
using SS.OAuth.Models;
using SS.OAuth.Models.Parameters;

namespace PPS.Endpoint.Helpers
{
    public static class TokenHelper
    {
        public static Creds GetTwoLegAccessToken( Creds user )
        {
            var requestToken      = GetRequestToken(user);
            var accessTokenParams = new AccessTokenParams(user, requestToken, null);
            var accessCmd         = new GetAccessTokenCommand(accessTokenParams);
            var accessToken       = accessCmd.GetToken();
            return accessToken;
        }

        public static Creds GetRequestToken( Creds consumer )
        {
            var requestTokenParams = new RequestTokenParams(consumer);
            var cmd                = new GetRequestTokenCommand(requestTokenParams);
            var requestToken       = cmd.GetToken();

            return requestToken;
        }
    }
}
