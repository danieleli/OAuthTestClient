namespace SS.OAuth
{
    public static partial class G
    {
        private static readonly string BASE_API_URL;
        private static readonly string BASE_SITE_URL;

        static G()
        {
            // todo: get from appsettings
            BASE_API_URL = "https://test.api.mxmerchant.com/v1";
            BASE_SITE_URL = "http://test.mxmerchant.com";
        }

        public static string BaseSiteUrl
        {
            get { return BASE_SITE_URL; }
        }

        public static string BaseApiUrl
        {
            get { return BASE_API_URL; }
        }
    }
}
