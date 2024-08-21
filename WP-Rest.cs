using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WP;

namespace WP
{
    public class Rest
    {
        public async Task CreatePostAsync(string wordpressUrl, string title, string content, string authToken)
        {
            var client = new HttpClient(); //Tworzy instancje HttpClient, który wysyła żądanie HTTP i usuwany jest po zakończeniu blogu 
            //Łączy URL z Wordpressem z resztą ścieżki, dodając tytuł i tekst.
            //Uri.EscapeDataString -> Koduje parametry w bezpieczny sposób dla URL
            string fullUrl = $"{wordpressUrl}/wp-json/wp/v2/posts?content={Uri.EscapeDataString(content)}&title={Uri.UnescapeDataString(title)}";
            var request = new HttpRequestMessage(HttpMethod.Post, fullUrl); //Tworzy wiadomość typu POST z podanym URL=em
            request.Headers.Add("Authorization", authToken); //Dodaje nagłówek z tokenem, by uwierzytelnić żadanie
            var response = await client.SendAsync(request); //Wysyła asynchroniczne żadanie HTTP, czekając na odpowiedź
            response.EnsureSuccessStatusCode(); //Sprawdza czy odpowiedź jest sukcesem, jak nie zgłasza wyjątek
            var responseBody = await response.Content.ReadAsStringAsync(); //Odczytuje odpowiedzi jako ciąg znaków
            Console.WriteLine(responseBody); // Wypisuje na konsoli
        }
    }

}
