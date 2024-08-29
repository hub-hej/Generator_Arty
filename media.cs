using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace WP
{
    public class MediaItem
    {
        public int id { get; set; }
        public string link { get; set; }
        public string title { get; set; }
        public string source_url { get; set; }
    }

    public class Media
    {
        private readonly HttpClient client;
        private readonly Configuration config;

        public Media(Configuration configuration)
        {
            client = new HttpClient();
            config = configuration;
        }

        public async Task<List<MediaItem>> GetMediaByFilenameAsync(string fileName)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{config.WordPressUrl}/wp-json/wp/v2/media?search={fileName}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", config.AuthToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var mediaItems = JsonConvert.DeserializeObject<List<MediaItem>>(responseBody);

            return mediaItems;
        }

        public async Task<int> UploadMediaAsync(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", filePath);
            }

            var fileName = Path.GetFileName(filePath);
            var fileExtension = Path.GetExtension(filePath).ToLowerInvariant();
            var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read);

            // Check if media already exists
            var existingMediaItems = await GetMediaByFilenameAsync(fileName);
            if (existingMediaItems.Count > 0)
            {
                Console.WriteLine($"Media with filename '{fileName}' already exists.");
                return existingMediaItems[0].id; // Return the ID of the existing media
            }

            using (var content = new MultipartFormDataContent())
            {
                var fileContent = new StreamContent(fileStream);

                string mimeType;

                switch (fileExtension)
                {
                    case ".jpg":
                    case ".jpeg":
                        mimeType = "image/jpeg";
                        break;
                    case ".png":
                        mimeType = "image/png";
                        break;
                    case ".gif":
                        mimeType = "image/gif";
                        break;
                    case ".bmp":
                        mimeType = "image/bmp";
                        break;
                    case ".tiff":
                        mimeType = "image/tiff";
                        break;
                    default:
                        mimeType = "application/octet-stream";
                        break;
                }

                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(mimeType);
                content.Add(fileContent, "file", fileName);

                var request = new HttpRequestMessage(HttpMethod.Post, $"{config.WordPressUrl}/wp-json/wp/v2/media");
                request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", config.AuthToken);
                request.Content = content;

                try
                {
                    var response = await client.SendAsync(request);
                    Console.WriteLine($"Status Code: {response.StatusCode}");
                    var responseBody = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Response: {responseBody}");

                    response.EnsureSuccessStatusCode();

                    var mediaItem = JsonConvert.DeserializeObject<MediaItem>(responseBody);

                    return mediaItem.id;
                }
                catch (HttpRequestException ex)
                {
                    Console.WriteLine($"HTTP Error: {ex.Message}");
                    throw;
                }
            }
        }
    }
}
