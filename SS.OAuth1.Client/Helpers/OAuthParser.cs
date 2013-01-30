using System.Collections.Generic;
using SS.OAuth1.Client.Parameters;

namespace SS.OAuth1.Client.Helpers
{
    public class OAuthParser
    {
        private SortedDictionary<string, string> GetOAuthParamsCore(OAuthParametersBase p)
        {
            var d = new SortedDictionary<string, string>();
            d.AddIfNotNullOrEmpty(Keys.NONCE, p.Nonce);
            d.AddIfNotNullOrEmpty(Keys.SIGNATURE_METHOD, Values.SIGNATURE_METHOD);
            d.AddIfNotNullOrEmpty(Keys.TIMESTAMP, p.Timestamp);
            d.AddIfNotNullOrEmpty(Keys.VERSION, Values.VERSION);
            d.AddIfNotNullOrEmpty(Keys.CONSUMER_KEY, p.Consumer.Key);
            return d;
        }

     
        public SortedDictionary<string, string> GetOAuthParamsNoSignature(OAuthParametersBase paramz, string callback = "", string token = "", string verifier = "")
        {
            var sortedDictionary = this.GetOAuthParamsCore(paramz);

            sortedDictionary.AddIfNotNullOrEmpty(Keys.CALLBACK, callback);
            sortedDictionary.AddIfNotNullOrEmpty(Keys.TOKEN, token);
            sortedDictionary.AddIfNotNullOrEmpty(Keys.VERIFIER, verifier);

            return sortedDictionary;
        }
    }
}