using SS.OAuth1.Client.Models;

namespace SS.OAuth1.Client
{
    public static class G
    {
        public const string BASE_API_URL = "https://test.api.mxmerchant.com/v1";
        public const string BASE_SITE_URL = "http://test.mxmerchant.com";

        public static class TestCreds
        {
            public static Creds DanUser = new Creds("dan+test", "cc2c0ef703029feddef7be3736aa8b53fc295ae7");
            public static Creds DanApp = new Creds("6rexu7a9ezawyss6krucyh9v", "oe793wU6pxvpSAUm7U1nF8Ol000=");
        }

    }
}