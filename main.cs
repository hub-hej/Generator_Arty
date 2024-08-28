using System;
using System.Threading.Tasks;
using WP;

public class Program
{
    public static async Task Main(string[] args)
    {
        var media = new Media();
        string wordpressUrl = "https://szybowanie.pl";
        string authToken = "YWt0ZTo0REoyIGJHeTkgcmVsRiBEc1d1IHFnWkYgYUFPVA==";
        string filePath = @"C:\Users\akte\Downloads\minionki.gif";

        try
        {
            int mediaId = await media.UploadMediaAsync(wordpressUrl, authToken, filePath);
            Console.WriteLine($"Media uploaded with ID: {mediaId}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
