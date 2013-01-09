using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SimpleMembership.OAuth
{
    public static class Credentials
    {
        public class Twitter
        {
            public static string KEY = ConfigurationManager.AppSettings["twitter_key"];
            public static string SECRET = ConfigurationManager.AppSettings["twitter_secret"];
        }

        public class Facebook
        {
            public static string KEY = ConfigurationManager.AppSettings["facebook_key"];
            public static string SECRET = ConfigurationManager.AppSettings["facebook_secret"];
        }

        public class MxMerchant
        {
            public static string KEY = ConfigurationManager.AppSettings["mxmerchant_key"];
            public static string SECRET = ConfigurationManager.AppSettings["mxmerchant_secret"];
        }
    }
}