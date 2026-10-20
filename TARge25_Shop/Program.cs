
using Microsoft.EntityFrameworkCore;
using TARge25_Shop.ApplicationServices.Services;
using TARge25_Shop.Core.ServiceInterface;
using TARge25_Shop.Data;
using TARge25Shop.ApplicationServices.Services;
using TARge25Shop.Core.ServiceInterface;

namespace TARge25_Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddScoped<ISpaceshipServices, SpaceshipServices>();

            builder.Services.AddScoped<IFileServices, FileServices>();

            builder.Services.AddScoped<IRealEstateServices, RealEstateServices>();

            builder.Services.AddScoped<IKindergartenServices, KindergartenServices>();

            // Cocktail API
            builder.Services.AddHttpClient<ICocktailServices, CocktailServices>();

            // OpenWeather API
            builder.Services.AddHttpClient<OpenWeatherService>();

            // On vaja alla laadida Microsoft.EntityFrameworkCore.SqlServer
            // NuGet pakett, et kasutada UseSqlServer meetodit
            builder.Services.AddDbContext<TARge25_ShopContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");

                // The default HSTS value is 30 days.
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.MapStaticAssets();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}")
                .WithStaticAssets();

            app.Run();
        }
    }
}
