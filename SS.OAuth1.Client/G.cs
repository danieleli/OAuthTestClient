using SS.OAuth1.Client.Helpers;

namespace SS.OAuth1.Client
{
    public static class G
    {
        public const string BASE_API_URL = "https://test.api.mxmerchant.com/v1";
        public const string BASE_SITE_URL = "http://test.mxmerchant.com";

        public static class TestCreds
        {
            private const string PASSWORD_SHA1 = "5ravvW12u10gQVQtfS4/rFuwVZM="; // password1234
            public static Creds DanUser = new Creds("dantest", PASSWORD_SHA1);
            public static Creds UserWithPlus = new Creds("dan+test", "cc2c0ef703029feddef7be3736aa8b53fc295ae7");
            public static Creds DanConsumer = new Creds("6rexu7a9ezawyss6krucyh9v", "oe793wU6pxvpSAUm7U1nF8Ol000=");
        }

    }
}