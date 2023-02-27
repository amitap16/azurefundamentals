using AzureTangyFunc.Models;
using Microsoft.EntityFrameworkCore;

namespace AzureTangyFunc.Data
{
    public class AzureTangyDbContext : DbContext
    {
        public AzureTangyDbContext(DbContextOptions<AzureTangyDbContext> dbContextOptions) : base(dbContextOptions) { }

        public DbSet<SalesRequest> SalesRequests { get; set; }
        public DbSet<GroceryItem> GroceryItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SalesRequest>(
                entity => { entity.HasKey(sr => sr.Id); }
                );

            modelBuilder.Entity<GroceryItem>(
                entity => { entity.HasKey(gi => gi.Id); }
                );
        }
    }
}