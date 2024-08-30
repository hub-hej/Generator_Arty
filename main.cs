﻿using System;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        string csvFilePath = @"C:\Users\akte\Downloads\Testowy.csv";
        var credentials = CsvReader.ReadCredentialsFromCsv(csvFilePath);

        foreach (var credential in credentials)
        {
            Console.WriteLine("Domain: " + credential.Domain);
            Console.WriteLine("Login: " + credential.Login);
            Console.WriteLine("Password: " + credential.Password);
            Console.WriteLine("API Key: " + credential.ApiKey);
            Console.WriteLine("Token Media: " + credential.TokenMedia);
            Console.WriteLine("Token Post: " + credential.TokenPost);
            Console.WriteLine("Additional Info: " + credential.AdditionalInfo);
            Console.WriteLine(new string('-', 50)); // Separator dla czytelności
        }

        Console.WriteLine("Naciśnij dowolny przycisk aby zamknąć konsole...");
        Console.ReadKey();
    }
}
