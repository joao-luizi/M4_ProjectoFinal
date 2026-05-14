using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using SharedLibrary;
using System;

namespace RepositoryLibrary.Seeds;

public class PaymentSeed
{
    public static async Task SeedPayments(IServiceProvider serviceProvider)
    {
        try
        {
            var em_context = serviceProvider.GetRequiredService<EM_DbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<EMUser>>();

            if (await em_context.UserPayments.AnyAsync())
                return;

            // Utilizadores demo criados pelo padrão:
            // customer.demo1@rideready.com
            // customer.demo2@rideready.com
            // customer.demo3@rideready.com

            var user1 = await userManager
                .FindByEmailAsync("student.demo1@rideready.com");

            var user2 = await userManager
                .FindByEmailAsync("student.demo2@rideready.com");

            var user3 = await userManager
                .FindByEmailAsync("student.demo3@rideready.com");

            if (user1 == null || user2 == null || user3 == null)
            {
                throw new Exception("Required demo users not found.");
            }

            var package1 = await em_context.Packages
                .FirstOrDefaultAsync(p => p.Id == 1);

            var package2 = await em_context.Packages
                .FirstOrDefaultAsync(p => p.Id == 2);

            var package3 = await em_context.Packages
                .FirstOrDefaultAsync(p => p.Id == 3);

            if (package1 == null || package2 == null || package3 == null)
            {
                throw new Exception("Required packages not found.");
            }

            DateOnly today = DateOnly.FromDateTime(DateTime.Today);

            var payments = new List<UserPayment>
        {
            // Pagamento válido
            new UserPayment
            {
                PackageId = package1.Id,
                UserId = user1.Id,
                BuyDate = today,
                DueDate = today.AddMonths(1)
            },

            // Pagamento quase a expirar
            new UserPayment
            {
                PackageId = package2.Id,
                UserId = user2.Id,
                BuyDate = today.AddDays(-25),
                DueDate = today.AddDays(5)
            },

            // Pagamento expirado
            new UserPayment
            {
                PackageId = package3.Id,
                UserId = user3.Id,
                BuyDate = today.AddMonths(-2),
                DueDate = today.AddMonths(-1)
            }
        };

            await em_context.UserPayments.AddRangeAsync(payments);
            await em_context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Error seeding payments.", e);
        }
    }

}
