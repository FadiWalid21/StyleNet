using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Services
{
    public class CartCleanupService(IServiceProvider serviceProvider) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await CleanupExpiredCartsAsync();

                await Task.Delay(TimeSpan.FromDays(1), stoppingToken);
            }
        }

        private async Task CleanupExpiredCartsAsync()
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var cartRepository = scope.ServiceProvider.GetRequiredService<ICartRepository>();

                var expiredCarts = await cartRepository.GetExpiredCartsAsync();

                foreach (var cart in expiredCarts)
                {
                    await cartRepository.DeleteCartAsync(cart.Id);
                }
            }
        }
    }
}
