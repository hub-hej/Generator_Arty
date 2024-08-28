using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using static Art;

namespace WP
{
    public class Rest
    {
        private readonly HttpClient client;

        public Rest()
        {
            client = new HttpClient();
        }

        public async Task<List<Category>> GetCategoriesAsync(string wordpressUrl, string authToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{wordpressUrl}/wp-json/wp/v2/categories?per_page=100");
            request.Headers.Add("Authorization", authToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<Category>>(responseBody);
            return categories;
        }

        public async Task<List<Tag>> GetTagsAsync(string wordpressUrl, string authToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{wordpressUrl}/wp-json/wp/v2/tags?per_page=100");
            request.Headers.Add("Authorization", authToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<Tag>>(responseBody);
            return tags;
        }

        public async Task<int> AddCategoryAsync(string wordpressUrl, string authToken, string categoryName)
        {
            var categoryData = new
            {
                name = categoryName,
                slug = categoryName.ToLower().Replace(" ", "-")
            };

            var json = JsonConvert.SerializeObject(categoryData);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{wordpressUrl}/wp-json/wp/v2/categories?per_page=100");
            request.Headers.Add("Authorization", authToken);
            request.Content = stringContent;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCategory = JsonConvert.DeserializeObject<Category>(responseBody);

            return createdCategory.id;
        }

        public async Task<int> AddTagAsync(string wordpressUrl, string authToken, string tagName)
        {
            var tagData = new
            {
                name = tagName,
                slug = tagName.ToLower().Replace(" ", "-")
            };

            var json = JsonConvert.SerializeObject(tagData);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{wordpressUrl}/wp-json/wp/v2/tags");
            request.Headers.Add("Authorization", authToken);
            request.Content = stringContent;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdTag = JsonConvert.DeserializeObject<Tag>(responseBody);

            return createdTag.id;
        }

        public async Task<int> CheckAndAddCategoryAsync(string wordpressUrl, string authToken, string categoryName)
        {
            if (!IsValidCategoryName(categoryName))
            {
                Console.WriteLine($"Błąd: Nieprawidłowa nazwa kategorii '{categoryName}'. Nazwa nie może zawierać cyfr.");
                return -1; // Zwróć -1 lub inne wartości oznaczające błąd
            }

            var categories = await GetCategoriesAsync(wordpressUrl, authToken);
            var existingCategory = categories.FirstOrDefault(c => c.name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (existingCategory != null)
            {
                Console.WriteLine($"Taka kategoria '{categoryName}' istnieje z ID: {existingCategory.id}");
                return existingCategory.id;
            }
            else
            {
                Console.WriteLine($"Kategoria '{categoryName}' nie istnieje. Tworzenie nowej kategorii...");
                var newCategoryId = await AddCategoryAsync(wordpressUrl, authToken, categoryName);
                if (newCategoryId == -1)
                {
                    Console.WriteLine($"Nie udało się stworzyć kategorii '{categoryName}'.");
                }
                else
                {
                    Console.WriteLine($"Nowa kategoria stworzona z ID: {newCategoryId}");
                }
                return newCategoryId;
            }
        }

        public async Task<int> CheckAndAddTagAsync(string wordpressUrl, string authToken, string tagName)
        {
            if (!IsValidTagName(tagName))
            {
                Console.WriteLine($"Błąd: Nieprawidłowa nazwa tagu '{tagName}'. Nazwa nie może zawierać cyfr.");
                return -1; // Zwróć -1 lub inne wartości oznaczające błąd
            }

            var tags = await GetTagsAsync(wordpressUrl, authToken);
            var existingTag = tags.FirstOrDefault(t => t.name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

            if (existingTag != null)
            {
                Console.WriteLine($"Taki tag '{tagName}' istnieje z ID: {existingTag.id}");
                return existingTag.id;
            }
            else
            {
                Console.WriteLine($"Tag '{tagName}' nie istnieje. Tworzenie nowego tagu...");
                var newTagId = await AddTagAsync(wordpressUrl, authToken, tagName);
                if (newTagId == -1)
                {
                    Console.WriteLine($"Nie udało się stworzyć tagu '{tagName}'.");
                }
                else
                {
                    Console.WriteLine($"Nowy tag stworzony z ID: {newTagId}");
                }
                return newTagId;
            }
        }


        private bool IsValidCategoryName(string categoryName)
        {
            // Sprawdź, czy kategoria zawiera liczby
            string pattern = @"\d";
            return !System.Text.RegularExpressions.Regex.IsMatch(categoryName, pattern);
        }

        private bool IsValidTagName(string tagName)
        {
            // Sprawdź, czy tag zawiera liczby
            string pattern = @"\d";
            return !System.Text.RegularExpressions.Regex.IsMatch(tagName, pattern);
        }

        public async Task CreatePostAsync(string wordpressUrl, string title, string content, string authToken, int[] categories, int[] tags)
        {

            if (categories == null || categories.Length == 0)
            {
                categories = new int[] { 1 }; // Bez kategorii
            }

            bool hasInvalidTag = false;
            if (tags == null || tags.Length == 0)
            {
                tags = new int[] { };
            }
            else
            {
                foreach (var tagId in tags)
                {
                    var tagsList = await GetTagsAsync(wordpressUrl, authToken);
                    if (!tagsList.Any(t => t.id == tagId && IsValidTagName(t.name)))
                    {
                        hasInvalidTag = true;
                        break;
                    }
                }
            }

            if (hasInvalidTag)
            {
                Console.WriteLine("Błąd: Jeden lub więcej tagów jest nieprawidłowych.");
                return;
            }

            var postData = new
            {
                title = title,
                content = content,
                status = "publish",
                categories = categories,
                tags = tags
            };

            var json = JsonConvert.SerializeObject(postData);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

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
