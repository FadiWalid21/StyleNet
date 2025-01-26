using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class ProductRepository(ApplicationDbContext dbContext) : IProductRepository
    {
        public void AddProduct(Product product)
        {
            dbContext.Products.Add(product);
        }

        public void DeleteProduct(Product product)
        {
            dbContext.Products.Remove(product);
        }

        public async Task<IReadOnlyList<string>> GetBrandsAsync()
        {
            return await dbContext.Products.Select(p => p.Brand)
            .Distinct()
            .ToListAsync();
        }

        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await dbContext.Products.FindAsync(id);
        }

        public async Task<IReadOnlyList<Product>> GetProductsAsync(string? brand, string? type, string? sort)
        {
            var query = dbContext.Products.AsQueryable();
            if (!string.IsNullOrWhiteSpace(brand))
                query = query.Where(p => p.Brand == brand);

            if (!string.IsNullOrWhiteSpace(type))
                query = query.Where(p => p.Type == type);

            query = sort switch
            {
                "priceAsc" => query.OrderBy(p => p.Price),
                "priceDesc" => query.OrderByDescending(p => p.Price),
                _ => query.OrderBy(p => p.Name)
            };


            return await query.ToListAsync();
        }

        public async Task<IReadOnlyList<string>> GetTypesAsync()
        {
            return await dbContext.Products.Select(p => p.Type)
            .Distinct()
            .ToListAsync();
        }

        public bool ProductExist(int id)
        {
            return dbContext.Products.Any(p => p.Id == id);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await dbContext.SaveChangesAsync() > 0;
        }

        public void UpdateProduct(Product product)
        {
            dbContext.Entry(product).State = EntityState.Modified;
        }
    }
}