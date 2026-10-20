
using Microsoft.AspNetCore.Mvc;
using TARge25_Shop.Core.ServiceInterface;
using TARge25_Shop.Models.Cocktail;
using TARge25Shop.Core.ServiceInterface;

namespace TARge25_Shop.Controllers
{
    public class CocktailController : Controller
    {
        private readonly ICocktailServices _cocktailServices;

        public CocktailController(ICocktailServices cocktailServices)
        {
            _cocktailServices = cocktailServices;
        }

        public async Task<IActionResult> Index(string searchName = "margarita")
        {
            var model = new CocktailIndexViewModel();

            model.SearchName = searchName;

            try
            {
                model.Cocktails = await _cocktailServices
                    .SearchCocktails(searchName);
            }
            catch (Exception)
            {
                ModelState.AddModelError("",
                    "Cocktail API andmete laadimine ebaõnnestus.");
            }

            return View(model);
        }
    }
}
