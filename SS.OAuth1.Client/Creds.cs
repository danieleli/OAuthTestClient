namespace SS.OAuth1.Client
{
    public class Creds
    {
        public string Key;
        public string Secret;

        public Creds(string key, string secret)
        {
            Key = key;
            Secret = secret;
        }

        public Creds Clone()
        {
            return new Creds(Key, Secret);
        }
    }

    public class ThreeLegCreds
    {
        public Creds User { get; set; }
        public Creds Consumer { get; set; }
    }

    public static class TestCreds
    {                                                                   
        public static ThreeLegCreds Dan = new ThreeLegCreds
            {
                User = new Creds("dan+test", "cc2c0ef703029feddef7be3736aa8b53fc295ae7"),
                Consumer = new Creds("6rexu7a9ezawyss6krucyh9v", "oe793wU6pxvpSAUm7U1nF8Ol000=")
            };
    }
}