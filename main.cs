using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using WP;

public class Art
{
    private static readonly Dictionary<string, int> Categories = new Dictionary<string, int>
    {
        { "Bez kategorii", 1 },
        { "Blog", 2 },
        { "Literatura", 3 },
        { "Ekrany akustyczne", 27 },
        { "Archiwizacja danych", 32 },
        { "Kunststoffzaune (Ogrodzenia plastikowe)", 92 },
        { "Ekrany dźwiękochłonne", 112 },
        { "Ekrany akustyczne cena", 118 },
        { "Informatyczna obsługa firm", 321 },
        { "Kompleksowa obsługa informatyczna", 416 },
    };
    public static async Task Main(string[] args)
    {
        WP.Rest rest = new WP.Rest();
        var art = new Art();
        string wordpressUrl = "https://esc2012-moscow.org/";
        string title = "Tsaddsa";
        string content = "Twolny";
        string authToken = "Basic YWt0ZToydm9EIHUwRTYgQVQ4RyB4ZGV1IGI5WFQgU2IwUA==";

        // Użytkownik wybiera kategorie
        int[] selectedCategories = { /* Categories["Blog"], Categories["Literatura"] && */ 92}; // Przykładowe wybrane kategorie
        int[] selectedTags = {  }; // Przykładowe ID tagów

        await rest.CreatePostAsync(wordpressUrl, title, content, authToken, selectedCategories, selectedTags);
    }
}