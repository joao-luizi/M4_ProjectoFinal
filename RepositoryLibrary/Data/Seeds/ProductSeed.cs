using Microsoft.Extensions.DependencyInjection;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Products.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Data.Seeds
{
    public static class ProductSeed
    {
        public static async Task SeedProductsAsync(IServiceProvider serviceProvider)
        {
            try
            {
                var context = serviceProvider.GetRequiredService<EM_DbContext>();

                if (!context.Products.Any())
                {
                    var products = new List<Product>
                {
                    new Product { Name = "Volteio 1x Semana", Price = 75.00m, IsActive = true },
                    new Product { Name = "Volteio 2x Semana", Price = 135.00m, IsActive = true },
                    new Product { Name = "Volteio 3x Semana", Price = 200.00m, IsActive = true },
                    new Product { Name = "Volteio Pack 8", Price = 157.00m, IsActive = true },

                    new Product { Name = "Sela 1 - 1x Semana", Price = 100.00m, IsActive = true },
                    new Product { Name = "Sela 1 - 2x Semana", Price = 175.00m, IsActive = true },
                    new Product { Name = "Sela 1 - 3x Semana", Price = 245.00m, IsActive = true },
                    new Product { Name = "Sela 1 - Pack 8", Price = 197.00m, IsActive = true },

                    new Product { Name = "Sela 2 - 1x Semana", Price = 125.00m, IsActive = true },
                    new Product { Name = "Sela 2 - 2x Semana", Price = 209.00m, IsActive = true },
                    new Product { Name = "Sela 2 - 3x Semana", Price = 290.00m, IsActive = true },
                    new Product { Name = "Sela 2 - Pack 8", Price = 226.00m, IsActive = true },

                    new Product { Name = "Individual 1x Semana", Price = 178.00m, IsActive = true },
                    new Product { Name = "Individual 2x Semana", Price = 295.00m, IsActive = true },
                    new Product { Name = "Individual 3x Semana", Price = 412.00m, IsActive = true },
                    new Product { Name = "Individual Pack", Price = 310.00m, IsActive = true },

                    new Product { Name = "Passeio Pack 1", Price = 80.00m, IsActive = true }
                };

                    await context.Products.AddRangeAsync(products);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error seeding products.", e);
            }
        }
    }
}
