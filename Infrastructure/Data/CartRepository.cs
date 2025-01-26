using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data
{
    public class CartRepository(ApplicationDbContext dbContext) : ICartRepository
    {
        public async Task<bool> DeleteCartAsync(string key)
        {
            var cart = await dbContext.ShoppingCarts.FindAsync(key);
            if (cart == null) return false;

            dbContext.ShoppingCarts.Remove(cart);
            await dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ShoppingCart?> GetCartAsync(string key)
        {
            var cart = await dbContext.ShoppingCarts.FindAsync(key);
            if (cart == null) return null;

            if (!string.IsNullOrEmpty(cart.ItemsJson))
            {
                cart.Items = JsonSerializer.Deserialize<List<CartItem>>(cart.ItemsJson) ?? new List<CartItem>();
            }

            return cart;
        }

        public async Task<ShoppingCart?> SetCartAsync(ShoppingCart cart)
        {
            cart.ItemsJson = JsonSerializer.Serialize(cart.Items);
            var existingCart = await dbContext.ShoppingCarts.FindAsync(cart.Id);
            if (existingCart != null)
            {
                dbContext.Entry(existingCart).CurrentValues.SetValues(cart);
                existingCart.ExpiresAt = DateTime.UtcNow.AddDays(10);
            }
            else
            {
                cart.CreatedAt = DateTime.UtcNow;
                cart.ExpiresAt = DateTime.UtcNow.AddDays(10);
                await dbContext.ShoppingCarts.AddAsync(cart);
            }

            await dbContext.SaveChangesAsync();
            return cart;
        }

        public async Task<List<ShoppingCart>> GetExpiredCartsAsync()
        {
            return await dbContext.ShoppingCarts
                .Where(cart => cart.ExpiresAt < DateTime.UtcNow)
                .ToListAsync();
        }
    }


}