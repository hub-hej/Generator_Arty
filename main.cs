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
    public class Category
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public class Tag
    {
        public int id { get; set; }
        public string name { get; set; }
    }
    public static async Task Main(string[] args)
    {
        WP.Rest rest = new WP.Rest();
        var art = new Art();
        string wordpressUrl = "https://szybowanie.pl/";
        string title = "Tsaddsa";
        string content = "Twolny";
        string authToken = "Basic YWt0ZTo0REoyIGJHeTkgcmVsRiBEc1d1IHFnWkYgYUFPVA==";

        // Sprawdzamy i dodajemy kategorię
        string newCategoryName = "drukarki i plotery";
        if (string.IsNullOrWhiteSpace(newCategoryName))
        {
            newCategoryName = "Bez kategorii";
        }
        int newCategoryId = await rest.CheckAndAddCategoryAsync(wordpressUrl, authToken, newCategoryName);

        // Sprawdzamy i dodajemy tag (tylko jeśli nie jest pusty)
        string newTagName = "bezpieczeństwo";
        int newTagId = -1;
        if (!string.IsNullOrWhiteSpace(newTagName))
        {
            newTagId = await rest.CheckAndAddTagAsync(wordpressUrl, authToken, newTagName);
        }

        if (newCategoryId != -1)
        {
            var categories = await rest.GetCategoriesAsync(wordpressUrl, authToken);
            var newCategory = categories.FirstOrDefault(c => c.id == newCategoryId);

            int[] selectedCategories = newCategory != null ? new int[] { newCategory.id } : new int[] { 1 };

            // Jeśli tag został poprawnie utworzony lub wybrany
            int[] selectedTags = newTagId != -1 ? new int[] { newTagId } : new int[] { };

            await rest.CreatePostAsync(wordpressUrl, title, content, authToken, selectedCategories, selectedTags);
        }
        else
        {
            Console.WriteLine("Nie udało się dodać kategorii i taga. Post nie zostanie utworzony. Naciśnij jakikolwiek przycisk by zamknąć konsole.");
            Console.ReadKey();
        }
    }
}