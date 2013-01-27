#region

using System;
using System.Net.Http;
using SS.OAuth1.Client.Parameters;
using log4net;

#endregion

namespace SS.OAuth1.Client.Composers
{
    public static class Util
    {

        private static readonly ILog LOG = LogManager.GetLogger(typeof(Util));

        public static Creds ExtractToken(HttpResponseMessage response)
        {
            var result = response.Content.ReadAsFormDataAsync().Result;

            if (result == null) throw new Exception("No Verifier Returned.");

            var key = result[AuthParameterFactory.Keys.TOKEN];
            var secret = result[AuthParameterFactory.Keys.TOKEN_SECRET];
            var token = new Creds(key, secret);

            LogCreds("Verifier", token);
            return token;
        }

        public static void LogCreds(string credType, Creds creds)
        {
            LOG.Info(credType + ": " + creds.Key);
            LOG.Info(credType + "Secret: " + creds.Secret);
        }

        public static void LogPair(string key, string value)
        {
            LOG.Info(key + ": " + value);
        }
    }

}
