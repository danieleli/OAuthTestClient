namespace MXM.API.Test.Controllers
{
    public class Creds
    {
        public string Key;
        public string Secret;
    }

    public class ThreeLedCreds
    {
        public Creds User { get; set; }
        public Creds Consumer { get; set; }
    }

    public static class TestCreds
    {
        // Two leg creds
        public static Creds TwoLegUser = new Creds { Key = "", Secret = "" };
        public static Creds ThreeLegUser = new Creds { Key = "", Secret = "" };
        public static Creds ThreeLegConsumer = new Creds { Key = "", Secret = "" };

        public static ThreeLedCreds TestingCreds = new ThreeLedCreds
        {
            User = ThreeLegUser,
            Consumer = ThreeLegConsumer
        };


        public static ThreeLedCreds Dan = new ThreeLedCreds
            {
                User = new Creds { Key = "dan+test", Secret = "cc2c0ef703029feddef7be3736aa8b53fc295ae7" },
                Consumer = new Creds { Key = "6rexu7a9ezawyss6krucyh9v", Secret = "oe793wU6pxvpSAUm7U1nF8Ol000=" }
            };
    }
}