using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WP;

class Program
{
    static async Task Main(string[] args)
    {
        string wordpressUrl = "https://szybowanie.pl";
        string authToken = "Basic YWt0ZTo0REoyIGJHeTkgcmVsRiBEc1d1IHFnWkYgYUFPVA==";

        var media = new Media();

        // Ścieżka do pliku obrazu
        string imagePath = @"C:\Users\akte\Downloads\cos.jpg";

        try
        {
            int mediaId = await media.UploadMediaAsync(wordpressUrl, authToken, imagePath);
            Console.WriteLine($"Załadowano obraz z ID: {mediaId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Wystąpił błąd: {ex.Message}");
        }

        // Dodaj tutaj kod do tworzenia postów, jeśli potrzebujesz
        // var art = new Art();
        // string title = "Tytuł";
        // string content = "Treść";
        // int[] categories = { 1 };
        // int[] tags = { 2 };
        // await art.CreatePostAsync(wordpressUrl, title, content, authToken, categories, tags);
    }
}
