using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WP
{

    public class Media
    {
        private readonly HttpClient client;

        public Media()
        {
            client = new HttpClient();
        }

        public async Task<int> UploadMediaAsync(string wordpressUrl, string authToken, string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            var fileName = Path.GetFileName(filePath);
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            using (var content = new MultipartFormDataContent())
            {
                var fileContent = new StreamContent(fileStream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("image/jpeg");
                content.Add(fileContent, "file", fileName);

                var request = new HttpRequestMessage(HttpMethod.Post, $"{wordpressUrl}/wp-json/wp/v2/media");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", authToken);
                request.Content = content;

                try
                {
                    var response = await client.SendAsync(request);
                    response.EnsureSuccessStatusCode();

                    var responseBody = await response.Content.ReadAsStringAsync();
                    var mediaItem = JsonConvert.DeserializeObject<MediaItem>(responseBody);

                    return mediaItem.id;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"Błąd HTTP: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
