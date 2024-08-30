using System;
using System.Collections.Generic;

public class Program
{
    public static void Main(string[] args)
    {
        string csvFilePath = @"C:\Users\akte\Downloads\Testowy.csv";
        var credentials = CsvReader.ReadCredentialsFromCsv(csvFilePath);

        foreach (var credential in credentials)
        {
            Console.WriteLine($"Domain: {credential.Domain}, Login: {credential.Login}, Password: {credential.Password}");
        }
    }
}
