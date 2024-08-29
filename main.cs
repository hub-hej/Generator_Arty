using System;
using System.Linq;
using System.Threading.Tasks;
using WP; // Upewnij się, że przestrzeń nazw WP jest używana

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = new Configuration(
            "https://szybowanie.pl",
            "YWt0ZTo0REoyIGJHeTkgcmVsRiBEc1d1IHFnWkYgYUFPVA=="
        );

        // Utworzenie instancji serwisów
        var mediaService = new Media(config);
        var restService = new Rest(config);

        // Przykładowe dane do posta
        string filePath = @"C:\Users\akte\Downloads\minionki.gif";
        string title = "Testowy Post";
        string content = "Treść testowego postu";

        try
        {
            // Upload media and get media ID
            int mediaId = await mediaService.UploadMediaAsync(filePath);
            Console.WriteLine($"Media uploaded with ID: {mediaId}");

            // Sprawdzanie i dodawanie kategorii oraz tagów
            string newCategoryName = "drukarki i plotery";
            int newCategoryId = await restService.CheckAndAddCategoryAsync(newCategoryName);

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
                int[] selectedTags = newTagId != -1 ? new int[] { newTagId } : new int[] { };

                // Tworzenie posta
                await restService.CreatePostAsync(title, content, selectedCategories, selectedTags);

                Console.WriteLine("Post created successfully.");
            }
            else
            {
                Console.WriteLine("Nie udało się dodać kategorii i taga. Post nie zostanie utworzony.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
