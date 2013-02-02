using SS.OAuth.Models;

namespace SS.OAuth
{
    public static partial class G
    {
        public const string CONSUMER_KEY = "consumer key";
        public const string CONSUMER_SECRET = "consumer secret";
        public static readonly Creds Consumer = new Creds(CONSUMER_KEY,CONSUMER_SECRET);

        public const string USER_KEY = "user key";
        public const string USER_SECRET = "user secret";
        public static readonly Creds User = new Creds(USER_KEY, USER_SECRET);

        private const string PASSWORD_SHA1 = "5ravvW12u10gQVQtfS4/rFuwVZM="; // password1234
        public static Creds DanUser = new Creds("dantest", PASSWORD_SHA1);
        public static Creds UserWithPlus = new Creds("dan+test", "cc2c0ef703029feddef7be3736aa8b53fc295ae7");
        
        // Might not be consumer - ???
        public static Creds DanTestAppConsumer = new Creds("6rexu7a9ezawyss6krucyh9v", "oe793wU6pxvpSAUm7U1nF8Ol000=");
    }
}
