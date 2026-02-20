using Microsoft.EntityFrameworkCore;
using System.Reflection;
using Talabat.Core.Entites.Brands;
using Talabat.Core.Entites.Categories;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Entites.Products;
using Talabat.Core.Entites.StockTransactions;

namespace Talabat.Repositaries.Data
{
    public class ShopDbContext : DbContext
    {
        public ShopDbContext(DbContextOptions<ShopDbContext> options) 
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Applyed All Configurations
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
        public DbSet<Brand> Brands { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<DelivaryMethod> DeliveryMethods { get; set; }
        public DbSet<OrderItems> OrderItems { get; set; }
        public DbSet<StockTransaction> StockTransactions { get; set; }
    }
}
