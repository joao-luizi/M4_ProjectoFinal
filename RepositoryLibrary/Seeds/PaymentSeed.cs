using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using SharedLibrary;

namespace RepositoryLibrary.Seeds;

public class PaymentSeed
{
    public static async Task SeedPayments(IServiceProvider serviceProvider)
    {
        try
        {
            var em_context = serviceProvider.GetRequiredService<EM_DbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<EMUser>>();
            EMUser Maria = await userManager.FindByEmailAsync("maria.ferreira@email.pt");
            EMUser Carlos = await userManager.FindByEmailAsync("carlos.santos@email.pt");
            EMUser Ana = await userManager.FindByEmailAsync("ana.costa@email.pt");
            var data = DateOnly.FromDateTime(DateTime.Now);
            if (!em_context.UserPayments.Any())
            {
                List<UserPayment> payments =
                [
                    new UserPayment { PackageId =1, UserId = Maria.Id, BuyDate = data, DueDate = data.AddMonths(1) },
                    new UserPayment { PackageId =2, UserId = Carlos.Id, BuyDate = data, DueDate = data.AddMonths(1) },
                    new UserPayment { PackageId =3, UserId = Ana.Id, BuyDate = data, DueDate = data.AddMonths(1) }
                ];
                await em_context.UserPayments.AddRangeAsync(payments);
                await em_context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error seeding payment", e);
        }
    }

}
