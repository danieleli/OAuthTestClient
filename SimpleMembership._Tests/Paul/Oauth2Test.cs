using System.Collections.Specialized;
using System.Web.Http;
using NUnit.Framework;
using Xunit.Sdk;
using log4net;

namespace MXM.API.Test.Controllers
{
    public static partial class OAuthRoutes
    {
        public static class V2
        {
            public const string ROUTE = G.BASE_URL + "/OAuth/2/";

            public const string ACCESS_TOKEN = ROUTE + "AccessToken";
            public const string AUTHORIZATION_CODE = ROUTE + "AuthorizationCode?client_id={0}&state={1}&redirect_uri={2}";
            public const string REFRESH_TOKEN = ROUTE + "TBD";
        }
    }

    [TestFixture]
    public class OAuth2Test
    {
       

    }


}
