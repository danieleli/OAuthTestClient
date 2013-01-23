namespace PPS.API.Constants
{
    public partial class OAuth
    {
        public partial class V1
        {
            public partial class Keys
            {
                //
                // List of know and used oauth parameters' names
                //    
                public const string CONSUMER = "oauth_consumer_key";
                public const string CALLBACK = "oauth_callback";
                public const string VERSION = "oauth_version";
                public const string SIGNATURE_METHOD = "oauth_signature_method";
                public const string SIGNATURE = "oauth_signature";
                public const string TIMESTAMP = "oauth_timestamp";
                public const string NONCE = "oauth_nonce";
                public const string TOKEN = "oauth_token";
                public const string TOKEN_SECRET = "oauth_token_secret";
                public const string VERIFIER = "oauth_verifier";
            }
        }
    }
}