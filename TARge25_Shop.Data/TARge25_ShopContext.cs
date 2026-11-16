
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TARge25_Shop.Core.Domain;

namespace TARge25_Shop.Data
{
    public class TARge25_ShopContext : IdentityDbContext<ApplicationUser>
    {
        public TARge25_ShopContext(
            DbContextOptions<TARge25_ShopContext> options)
            : base(options)
        {
        }

        // Spaceship
        public DbSet<Spaceship> Spaceships { get; set; }

        // Files API
        public DbSet<FileToApi> FileToApis { get; set; }

        // RealEstate
        public DbSet<RealEstate> RealEstates { get; set; }

        // Files Database
        public DbSet<FileToDatabase> FileToDatabases { get; set; }

        // Kindergarten
        public DbSet<Kindergarten> Kindergartens { get; set; }
    }
}
