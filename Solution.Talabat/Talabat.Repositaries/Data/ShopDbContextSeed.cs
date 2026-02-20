using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entites.Brands;
using Talabat.Core.Entites.Categories;
using Talabat.Core.Entites.Orders;
using Talabat.Core.Entites.Products;

namespace Talabat.Repositaries.Data
{
    public static class ShopDbContextSeed
    {
        public static async Task SeedAsync(ShopDbContext _dbContext)
        {
            // Set Brand

            if(!_dbContext.Set<Brand>().Any()) // Brands Is Empty
            {
                var data = File.ReadAllText("../Talabat.Repositaries/Data/DataSeeding/Brands.JSON");
                var brands = JsonSerializer.Deserialize<List<Brand>>(data);

                //if (brands is not null && brands.Any()) {
                if (brands?.Any() == true) {
                    await _dbContext.Set<Brand>().AddRangeAsync(brands);
                    await _dbContext.SaveChangesAsync();
                }
            }

            // Set Category

            if (!_dbContext.Set<Category>().Any())
            {
                var data = File.ReadAllText("../Talabat.Repositaries/Data/DataSeeding/Categories.JSON");
                var category = JsonSerializer.Deserialize<List<Category>>(data);
                if(category?.Any() == true)
                {
                    await _dbContext.Set<Category>().AddRangeAsync(category);
                    await _dbContext.SaveChangesAsync();
                }
            }

            // Set Product

            if (!_dbContext.Set<Product>().Any())
            {
                var data = File.ReadAllText("../Talabat.Repositaries/Data/DataSeeding/Products.JSON");
                var products = JsonSerializer.Deserialize<List<Product>>(data);
                if (products?.Any() == true) { 
                    await _dbContext.Set<Product>().AddRangeAsync(products);
                    await _dbContext.SaveChangesAsync();
                }
            }

            // Set Delivary Method
            if(!_dbContext.Set<DelivaryMethod>().Any())
            {
                var data = File.ReadAllText("../Talabat.Repositaries/Data/DataSeeding/delivery.JSON");
                var delivaryData = JsonSerializer.Deserialize<List<DelivaryMethod>>(data);
                if(delivaryData?.Any() == true)
                {
                    await _dbContext.AddRangeAsync(delivaryData);
                    await _dbContext.SaveChangesAsync();
                }
            }
        }

    }
}
