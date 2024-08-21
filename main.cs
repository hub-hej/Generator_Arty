using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WP;

public class Art
{
    public static async Task Main(string[] args)
    {
        WP.Rest rest = new WP.Rest();
        var art = new Art();
        string wordpressUrl = "https://esc2012-moscow.org/";
        string title = "Tsaddsa";
        string content = "Twolny";
        string authToken = "Basic YWt0ZToydm9EIHUwRTYgQVQ4RyB4ZGV1IGI5WFQgU2IwUA==";

        await rest.CreatePostAsync(wordpressUrl, title, content, authToken); //Metoda, która tworzy post na Wordpressie
    }
}