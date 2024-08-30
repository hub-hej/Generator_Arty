public class WordPressCredentials
{
    public string Domain { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }

    public WordPressCredentials(string domain, string login, string password)
    {
        Domain = domain;
        Login = login;
        Password = password;
    }
}
