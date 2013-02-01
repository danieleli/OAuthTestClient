using System;
using System.Globalization;

namespace SS.OAuth.Misc
{
    public static class Utils
    {
        private static readonly Random _Random = new Random();

        public static string GenerateTimeStamp()
        {
            // Default implementation of UNIX time of the current UTC time
            var ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        }

        public static string GenerateNonce()
        {
            // Just a simple implementation of a random number between 123400 and 9999999
            return _Random.Next(123400, 9999999).ToString(CultureInfo.InvariantCulture);
        }
    }
}