
using TARge25_Shop.Core.Dto;
using TARge25Shop.Core.Dto;

namespace TARge25_Shop.Models.Cocktail
{
    public class CocktailIndexViewModel
    {
        public string SearchName { get; set; } = "margarita";

        public List<CocktailDto> Cocktails { get; set; }
            = new List<CocktailDto>();
    }
}
