using SS.OAuth1.Client.Helpers;
using log4net;

namespace SS.OAuth1.Client.Extensions
{
    public static class LogExtensions
    {
        public static void LogCreds(this ILog logger, string credType, Creds creds)
        {
            logger.Info("------ Credentials -----");
            logger.Info(credType + ": " + creds.Key);
            logger.Info(credType + "Secret: " + creds.Secret);
            logger.Info("------------------------");
        }

        public static void LogPair(this ILog logger, string key, string value)
        {
            logger.Info(key + ": " + value);
        }
    }
}