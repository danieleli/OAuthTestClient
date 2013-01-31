using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;

namespace SS.OAuth1.Client.Extensions
{
    public static class NameValueCollectionEx
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(NameValueCollectionEx));

        public static string Stringify(this NameValueCollection collection)
        {
            /*
            
            3.5.1.  Authorization Header

            Protocol parameters can be transmitted using the HTTP "Authorization"
            header field as defined by [RFC2617] with the auth-scheme name set to
            "OAuth" (case insensitive).

            For example:

                Authorization: OAuth realm="Example",
                oauth_consumer_key="0685bd9184jfhq22",
                oauth_token="ad180jjd733klru7",
                oauth_signature_method="HMAC-SHA1",
                oauth_signature="wOJIO9A2W5mFwDgiDvZbTSMK%2FPY%3D",
                oauth_timestamp="137131200",
                oauth_nonce="4572616e48616d6d65724c61686176",
                oauth_version="1.0"

            Protocol parameters SHALL be included in the "Authorization" header
            field as follows:

            1.  Parameter names and values are encoded per Parameter Encoding
                (Section 3.6). http://tools.ietf.org/html/rfc5849#section-3.6

            2.  Each parameter's name is immediately followed by an "=" character
                (ASCII code 61), a """ character (ASCII code 34), the parameter
                value (MAY be empty), and another """ character (ASCII code 34).

            3.  Parameters are separated by a "," character (ASCII code 44) and
                OPTIONAL linear whitespace per [RFC2617].

            4.  The OPTIONAL "realm" parameter MAY be added and interpreted per
                [RFC2617] section 1.2. http://tools.ietf.org/html/rfc2617#section-1.2

            Servers MAY indicate their support for the "OAuth" auth-scheme by
            returning the HTTP "WWW-Authenticate" response header field upon
            client requests for protected resources.  As per [RFC2617], such a
            response MAY include additional HTTP "WWW-Authenticate" header
            fields 
             
            */

            var sb = new StringBuilder();
            var isFirstItem = true;

            var sortedKeys = SortKeys(collection.Keys);


            foreach (var key in sortedKeys)
            {
                var values = collection.GetValues(key) ?? new string[] {null};
                var orderValues = values.OrderBy(s => s);
                foreach (var orderValue in orderValues)
                {
                    if (!isFirstItem)
                    {
                        sb.Append(",");
                    }
                    sb.Append(string.Format("{0}=\"{1}\"", key.UrlEncodeForOAuth(), orderValue.UrlEncodeForOAuth()));
                    isFirstItem = false;   
                }
                
            }

            return sb.ToString();
        }

        private static IEnumerable<string> SortKeys(NameObjectCollectionBase.KeysCollection keys)
        {
            var sortedList = new SortedSet<string>();
            foreach (var key in keys)
            {
                sortedList.Add(key.ToString());
            }
            return sortedList;
        }

        public static void AddIfNotNullOrEmpty(this NameValueCollection collection, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                collection.Add(key, value);
            }
        }
    }
}