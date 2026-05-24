using Microsoft.Extensions.DependencyInjection;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Seeds
{
    public static class EntitlementSeed
    {
        public static async Task SeedEntitlementsAsync(IServiceProvider serviceProvider)
        {
            try
            {
                var context = serviceProvider.GetRequiredService<EM_DbContext>();

                if (!context.ProductEntitlements.Any())
                {
                    var productEntitlements = new List<ProductEntitlement>
                    {
                                    // Volteio
                new ProductEntitlement { ProductId = 1, LessonTypeId = 1, WeeklyFrequency = 1 },
                new ProductEntitlement { ProductId = 2, LessonTypeId = 1, WeeklyFrequency = 2 },
                new ProductEntitlement { ProductId = 3, LessonTypeId = 1, WeeklyFrequency = 3 },
                new ProductEntitlement { ProductId = 4, LessonTypeId = 1, CreditsGranted = 8 },

                // Sela 1
                new ProductEntitlement { ProductId = 5, LessonTypeId = 2, WeeklyFrequency = 1 },
                new ProductEntitlement { ProductId = 6, LessonTypeId = 2, WeeklyFrequency = 2 },
                new ProductEntitlement { ProductId = 7, LessonTypeId = 2, WeeklyFrequency = 3 },
                new ProductEntitlement { ProductId = 8, LessonTypeId = 2, CreditsGranted = 8 },

                // Sela 2
                new ProductEntitlement { ProductId = 9, LessonTypeId = 3, WeeklyFrequency = 1 },
                new ProductEntitlement { ProductId = 10, LessonTypeId = 3, WeeklyFrequency = 2 },
                new ProductEntitlement { ProductId = 11, LessonTypeId = 3, WeeklyFrequency = 3 },
                new ProductEntitlement { ProductId = 12, LessonTypeId = 3, CreditsGranted = 8 },

                // Individual
                new ProductEntitlement { ProductId = 13, LessonTypeId = 4, WeeklyFrequency = 1 },
                new ProductEntitlement { ProductId = 14, LessonTypeId = 4, WeeklyFrequency = 2 },
                new ProductEntitlement { ProductId = 15, LessonTypeId = 4, WeeklyFrequency = 3 },
                new ProductEntitlement { ProductId = 16, LessonTypeId = 4, CreditsGranted = 8 },

                // Passeio
                new ProductEntitlement { ProductId = 17, LessonTypeId = 5, CreditsGranted = 1 }

                    };

                    await context.ProductEntitlements.AddRangeAsync(productEntitlements);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error seeding Entitlements.", e);
            }
        }
    }
}
