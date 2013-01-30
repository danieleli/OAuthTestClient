namespace SS.OAuth1.Client.Helpers
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
}