namespace WP
{
    public class Configuration
    {
        public string WordPressUrl { get; }
        public string AuthToken { get; }

        public Configuration(string wordpressUrl, string authToken)
        {
            WordPressUrl = wordpressUrl;
            AuthToken = authToken;
        }
    }
}
