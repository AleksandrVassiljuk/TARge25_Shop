
using System.Text.Json;
using TARge25Shop.Core.Dto;
using TARge25Shop.Core.ServiceInterface;

namespace TARge25Shop.ApplicationServices.Services
{
    public class CocktailServices : ICocktailServices
    {
        private readonly HttpClient _httpClient;

        public CocktailServices(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CocktailDto>> SearchCocktails(string name)
        {
            string url = "https://www.thecocktaildb.com/api/json/v1/1/search.php?s="
                + Uri.EscapeDataString(name);

            var response = await _httpClient.GetAsync(url);

            response.EnsureSuccessStatusCode();

            string json = await response.Content.ReadAsStringAsync();

            using JsonDocument document = JsonDocument.Parse(json);

            var cocktails = new List<CocktailDto>();

            if (document.RootElement.GetProperty("drinks").ValueKind
                == JsonValueKind.Null)
            {
                return cocktails;
            }

            foreach (var drink in document.RootElement
                .GetProperty("drinks").EnumerateArray())
            {
                cocktails.Add(new CocktailDto
                {
                    IdDrink = GetValue(drink, "idDrink"),
                    StrDrink = GetValue(drink, "strDrink"),
                    StrDrinkThumb = GetValue(drink, "strDrinkThumb"),
                    StrCategory = GetValue(drink, "strCategory"),
                    StrInstructions = GetValue(drink, "strInstructions"),
                    StrIngredient1 = GetValue(drink, "strIngredient1"),
                    StrIngredient2 = GetValue(drink, "strIngredient2"),
                    StrIngredient3 = GetValue(drink, "strIngredient3"),
                    StrIngredient4 = GetValue(drink, "strIngredient4")
                });
            }

            return cocktails;
        }

        private static string GetValue(JsonElement drink, string property)
        {
            if (drink.TryGetProperty(property, out var value))
            {
                return value.GetString() ?? "";
            }

            return "";
        }
    }
}
