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

            // Sprawdzenie nagłówków
            var expectedHeaders = new string[] { "Domena", "Login WP", "Haslo WP", "API Key (Wordpress)", "Token (Visual Studio i Postman) = Media", "Token (Visual Studio i Postman) = Post", "Dodatkowe informacje" };
            var headers = lines[0].Split(',');

            if (headers.Length != expectedHeaders.Length)
            {
                throw new Exception("Struktura pliku CSV jest nieprawidłowa: Niewłaściwa liczba kolumn. Domyślna liczba kolumn to 7 [Domena, Login WP, Haslo WP, API Key (Wordpress), Token (Visual Studio i Postman) = Media, Token (Visual Studio i Postman) = Post, Dodatkowe informacje]");
            }

            for (int i = 0; i < headers.Length; i++)
            {
                if (headers[i].Trim() != expectedHeaders[i])
                {
                    throw new Exception($"Struktura pliku CSV jest nieprawidłowa: Oczekiwano nagłówka '{expectedHeaders[i]}', ale znaleziono '{headers[i]}'.");
                }
            }

            // Przetwarzanie danych (pomijamy nagłówki)
            for (int i = 1; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                if (values.Length == expectedHeaders.Length)
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
                else
                {
                    Console.WriteLine($"Ostrzeżenie: Wiersz {i + 1} ma niewłaściwą liczbę kolumn. Zostanie pominięty.");
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
