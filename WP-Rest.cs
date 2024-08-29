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
        private readonly Configuration config;

        public Rest(Configuration configuration)
        {
            client = new HttpClient();
            config = configuration;
        }

        public async Task<List<Category>> GetCategoriesAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{config.WordPressUrl}/wp-json/wp/v2/categories?per_page=100");
            request.Headers.Add("Authorization", config.AuthToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var categories = JsonConvert.DeserializeObject<List<Category>>(responseBody);
            return categories;
        }

        public async Task<List<Tag>> GetTagsAsync()
        {
            var request = new HttpRequestMessage(HttpMethod.Get, $"{config.WordPressUrl}/wp-json/wp/v2/tags?per_page=100");
            request.Headers.Add("Authorization", config.AuthToken);

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var tags = JsonConvert.DeserializeObject<List<Tag>>(responseBody);
            return tags;
        }

        public async Task<int> AddCategoryAsync(string categoryName)
        {
            var categoryData = new
            {
                name = categoryName,
                slug = categoryName.ToLower().Replace(" ", "-")
            };

            var json = JsonConvert.SerializeObject(categoryData);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{config.WordPressUrl}/wp-json/wp/v2/categories?per_page=100");
            request.Headers.Add("Authorization", config.AuthToken);
            request.Content = stringContent;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdCategory = JsonConvert.DeserializeObject<Category>(responseBody);

            return createdCategory.id;
        }

        public async Task<int> AddTagAsync(string tagName)
        {
            var tagData = new
            {
                name = tagName,
                slug = tagName.ToLower().Replace(" ", "-")
            };

            var json = JsonConvert.SerializeObject(tagData);
            var stringContent = new StringContent(json, Encoding.UTF8, "application/json");

            var request = new HttpRequestMessage(HttpMethod.Post, $"{config.WordPressUrl}/wp-json/wp/v2/tags");
            request.Headers.Add("Authorization", config.AuthToken);
            request.Content = stringContent;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            var createdTag = JsonConvert.DeserializeObject<Tag>(responseBody);

            return createdTag.id;
        }

        public async Task<int> CheckAndAddCategoryAsync(string categoryName)
        {
            if (!IsValidCategoryName(categoryName))
            {
                Console.WriteLine($"Błąd: Nieprawidłowa nazwa kategorii '{categoryName}'. Nazwa nie może zawierać cyfr.");
                return -1;
            }

            var categories = await GetCategoriesAsync();
            var existingCategory = categories.FirstOrDefault(c => c.name.Equals(categoryName, StringComparison.OrdinalIgnoreCase));

            if (existingCategory != null)
            {
                Console.WriteLine($"Taka kategoria '{categoryName}' istnieje z ID: {existingCategory.id}");
                return existingCategory.id;
            }
            else
            {
                Console.WriteLine($"Kategoria '{categoryName}' nie istnieje. Tworzenie nowej kategorii...");
                var newCategoryId = await AddCategoryAsync(categoryName);
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

        public async Task<int> CheckAndAddTagAsync(string tagName)
        {
            if (!IsValidTagName(tagName))
            {
                Console.WriteLine($"Błąd: Nieprawidłowa nazwa tagu '{tagName}'. Nazwa nie może zawierać cyfr.");
                return -1;
            }

            var tags = await GetTagsAsync();
            var existingTag = tags.FirstOrDefault(t => t.name.Equals(tagName, StringComparison.OrdinalIgnoreCase));

            if (existingTag != null)
            {
                Console.WriteLine($"Taki tag '{tagName}' istnieje z ID: {existingTag.id}");
                return existingTag.id;
            }
            else
            {
                Console.WriteLine($"Tag '{tagName}' nie istnieje. Tworzenie nowego tagu...");
                var newTagId = await AddTagAsync(tagName);
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
            string pattern = @"\d";
            return !System.Text.RegularExpressions.Regex.IsMatch(categoryName, pattern);
        }

        private bool IsValidTagName(string tagName)
        {
            string pattern = @"\d";
            return !System.Text.RegularExpressions.Regex.IsMatch(tagName, pattern);
        }

        public async Task CreatePostAsync(string title, string content, int[] categories, int[] tags)
        {
            if (categories == null || categories.Length == 0)
            {
                categories = new int[] { 1 };
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
                    var tagsList = await GetTagsAsync();
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

            string fullUrl = $"{config.WordPressUrl}/wp-json/wp/v2/posts";

            var request = new HttpRequestMessage(HttpMethod.Post, fullUrl);
            request.Headers.Add("Authorization", config.AuthToken);
            request.Content = stringContent;

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            Console.WriteLine(responseBody);
        }
    }
}
