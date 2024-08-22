using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WP;

namespace WP
{
    public class Rest
    {
        public async Task CreatePostAsync(string wordpressUrl, string title, string content, string authToken, int[] categories, int[] tags)
        {
            var client = new HttpClient(); //Tworzy instancje HttpClient, który wysyła żądanie HTTP i usuwany jest po zakończeniu blogu 

            // Tworzymy obiekt z danymi do wysłania
            var postData = new
            {
                title = title,
                content = content,
                status = "publish", // Określa, że post ma być opublikowany od razu
                categories = categories, // Przypisanie wybranych kategorii do posta
                tags = tags // Przypisanie wybranych tagów do posta
            };

            // Serializujemy obiekt do JSON-a
            var json = JsonConvert.SerializeObject(postData);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            // Tworzymy pełny URL dla żądania POST
            string fullUrl = $"{wordpressUrl}/wp-json/wp/v2/posts";

            var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
            request.Headers.Add("Authorization", authToken);
            request.Content = stringContent;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }
    }
}
