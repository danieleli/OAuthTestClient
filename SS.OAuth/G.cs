namespace SS.OAuth
{
    public static partial class G
    {
        private static readonly string _BaseApiUrl;
        private static readonly string _BaseSiteUrl;


        public static string BaseSiteUrl
        {
            get { return _BaseSiteUrl; }
        }

        public static string BaseApiUrl
        {
            get { return _BaseApiUrl; }
            
        }

        static G()
        {
            // todo: get from appsettings
            _BaseApiUrl = "https://test.api.mxmerchant.com/v1";
            _BaseSiteUrl = "http://test.mxmerchant.com";
        }
    }
}