
using TARge25Shop.Core.Dto;

namespace TARge25Shop.Core.ServiceInterface
{
    public interface ICocktailServices
    {
        Task<List<CocktailDto>> SearchCocktails(string name);
    }
}
