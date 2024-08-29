using System;
using System.Threading.Tasks;
using WP;

public class Program
{
    public static async Task Main(string[] args)
    {
        var config = new Configuration(
            "https://szybowanie.pl",
            "YWt0ZTo0REoyIGJHeTkgcmVsRiBEc1d1IHFnWkYgYUFPVA=="
        );

        var mediaService = new Media(config);
        string filePath = @"C:\Users\akte\Downloads\minionki.gif";

        try
        {
            int mediaId = await mediaService.UploadMediaAsync(filePath);
            Console.WriteLine($"Media uploaded with ID: {mediaId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
