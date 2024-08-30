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
            for (int i = 1; i < lines.Length; i++) // Rozpocznij od drugiej linii (indeks 1), aby pominąć nagłówki
            {
                var values = lines[i].Split(',');
                if (values.Length >= 7)
                {
                    var credential = new WordPressCredentials(
                        values[0], // Domena 
                        values[1], // Login WP
                        values[2], // Hasło WP
                        values[3], // API Key (Wordpress) 
                        values[4], // Token (Visual Studio i Postman) → Media
                        values[5], // Token (Visual Studio i Postman) → Post
                        values[6]  // Dodatkowe informacje
                    );
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
