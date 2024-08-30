using System;
using System.Collections.Generic;
using System.IO;

public class CsvReader
{
    public static bool IsFileLocked(string filePath)
    {
        try
        {
            using (FileStream stream = File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.None))
            {
                stream.Close();
            }
        }
        catch (IOException)
        {
            return true; // Plik jest zablokowany
        }

        return false; // Plik nie jest zablokowany
    }

    public static List<WordPressCredentials> ReadCredentialsFromCsv(string filePath)
    {
        var credentials = new List<WordPressCredentials>();

        if (IsFileLocked(filePath))
        {
            Console.WriteLine("Plik jest obecnie używany przez inny proces. Zamknij plik i spróbuj ponownie.");
            return credentials;
        }

        try
        {
            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines)
            {
                var values = line.Split(',');
                if (values.Length >= 3)
                {
                    var credential = new WordPressCredentials(values[0], values[1], values[2]);
                    credentials.Add(credential);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Błąd podczas odczytu pliku: {ex.Message}");
        }

        return credentials;
    }
}
