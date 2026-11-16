
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TARge25_Shop.ApplicationServices.Services;
using TARge25_Shop.Core.Domain;
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

            // MVC
            builder.Services.AddControllersWithViews();

            // Spaceship
            builder.Services.AddScoped<ISpaceshipServices, SpaceshipServices>();

            // File
            builder.Services.AddScoped<IFileServices, FileServices>();

            // RealEstate
            builder.Services.AddScoped<IRealEstateServices, RealEstateServices>();

            // Kindergarten
            builder.Services.AddScoped<IKindergartenServices, KindergartenServices>();

            // Cocktail API
            builder.Services.AddHttpClient<ICocktailServices, CocktailServices>();

            // OpenWeather API
            builder.Services.AddHttpClient<OpenWeatherService>();

            // Email
            builder.Services.AddScoped<IEmailServices, EmailServices>();

            // SQL Server
            builder.Services.AddDbContext<TARge25_ShopContext>(options =>
                options.UseSqlServer(
                    builder.Configuration.GetConnectionString("DefaultConnection")));

            // ASP.NET Core Identity
            builder.Services
                .AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequireLowercase = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequiredLength = 6;

                    options.User.RequireUniqueEmail = true;
                })
                .AddEntityFrameworkStores<TARge25_ShopContext>()
                .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/Login";
            });

            var app = builder.Build();

            // Error handling
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            // Identity authentication
            app.UseAuthentication();
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
