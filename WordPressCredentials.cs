public class WordPressCredentials
{
    public string Domain { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string ApiKey { get; set; }
    public string TokenMedia { get; set; }
    public string TokenPost { get; set; }
    public string AdditionalInfo { get; set; }

    public WordPressCredentials(string domain, string login, string password, string apiKey, string tokenMedia, string tokenPost, string additionalInfo)
    {
        Domain = domain;
        Login = login;
        Password = password;
        ApiKey = apiKey;
        TokenMedia = tokenMedia;
        TokenPost = tokenPost;
        AdditionalInfo = additionalInfo;
    }
}
