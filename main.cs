using System;
using System.Linq;
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
        var config = new Configuration(
            "https://szybowanie.pl",
            "Basic YWt0ZTo0REoyIGJHeTkgcmVsRiBEc1d1IHFnWkYgYUFPVA=="
        );

        var restService = new Rest(config);

        string title = "Test";
        string content = "Dowolny tekst przykładowy";

        // Sprawdzamy i dodajemy kategorię
        string newCategoryName = "drukarki i plotery";
        if (string.IsNullOrWhiteSpace(newCategoryName))
        {
            newCategoryName = "Bez kategorii";
        }
        int newCategoryId = await restService.CheckAndAddCategoryAsync(newCategoryName);

        // Sprawdzamy i dodajemy tag (tylko jeśli nie jest pusty)
        string newTagName = "bezpieczeństwo";
        int newTagId = -1;
        if (!string.IsNullOrWhiteSpace(newTagName))
        {
            newTagId = await restService.CheckAndAddTagAsync(newTagName);
        }

        if (newCategoryId != -1)
        {
            var categories = await restService.GetCategoriesAsync();
            var newCategory = categories.FirstOrDefault(c => c.id == newCategoryId);

            int[] selectedCategories = newCategory != null ? new int[] { newCategory.id } : new int[] { 1 };

            // Jeśli tag został poprawnie utworzony lub wybrany
            int[] selectedTags = newTagId != -1 ? new int[] { newTagId } : new int[] { };

            await restService.CreatePostAsync(title, content, selectedCategories, selectedTags);
        }
        else
        {
            Console.WriteLine("Nie udało się dodać kategorii i taga. Post nie zostanie utworzony. Naciśnij jakikolwiek przycisk by zamknąć konsole.");
            Console.ReadKey();
        }
    }
}
