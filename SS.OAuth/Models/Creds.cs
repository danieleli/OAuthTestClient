namespace SS.OAuth.Models
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
    }
}