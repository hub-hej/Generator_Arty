namespace WP
{
    public class Configuration
    {
        public string WordPressUrl { get; set; }
        public string AuthToken { get; set; }

        public Configuration(string wordpressUrl, string authToken)
        {
            WordPressUrl = wordpressUrl;
            AuthToken = authToken;
        }
    }
}
