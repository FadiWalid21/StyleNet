using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Core.Entities;

namespace Infrastructure.Data
{
    public class StoreContextSeed
    {
        public static async Task SeedAysnc(ApplicationDbContext dbContext)
        {
            if(!dbContext.Products.Any()){
                var productData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/products.json");
                var products = JsonSerializer.Deserialize<List<Product>>(productData);

                if(products is null) return;
                await dbContext.AddRangeAsync(products);
                await dbContext.SaveChangesAsync();
            }

            if(!dbContext.DeliveryMethods.Any()){
                var dmData = await File.ReadAllTextAsync("../Infrastructure/Data/SeedData/delivery.json");
                var methods = JsonSerializer.Deserialize<List<DeliveryMethod>>(dmData);

                if(methods is null) return;
                await dbContext.AddRangeAsync(methods);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}