using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using SharedLibrary;

namespace RepositoryLibrary.Seeds;

public static class HorseSeed
{

    public static async Task SeedHorses(IServiceProvider serviceProvider)
    {
        try
        {
            var em_context = serviceProvider.GetRequiredService<EM_DbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<EMUser>>();
            var school = await em_context.Schools.FirstOrDefaultAsync(s => s.SchoolId == 1);
            if (!em_context.Horses.Any())
            {
                List<Horse> horses =
                [
                new Horse{
                    Name = "Relâmpago",
                    Breed = "Mangalarga Marchador",
                    Age = 6,
                    School = school
                },
                new Horse{
                    Name = "Estrela",
                    Breed = "Puro-Sangue Lusitano",
                    Age = 4,
                    School = school
                },
                new Horse{
                    Name = "Ventania",
                    Breed = "Crioulo",
                    Age = 7,
                    School = school
                },
                new Horse{
                    Name = "Fumaca",
                    Breed = "Quarto de Milha",
                    Age = 5,
                    School = school
                },
                new Horse{
                    Name = "Brisa",
                    Breed = "Árabe",
                    Age = 3,
                    School = school
                }];

                await em_context.AddRangeAsync(horses);
                await em_context.SaveChangesAsync();
                var fumaca = await em_context.Horses.FirstOrDefaultAsync(h => h.Name == "Fumaca");
                var brisa = await em_context.Horses.FirstOrDefaultAsync(h => h.Name == "Brisa");
                var users = await userManager.Users.Where(u => u.FirstName == "Maria" || u.FirstName == "Ana").ToListAsync();
                await em_context.UserHorses.AddAsync(new UserHorse { Horse = fumaca, Relationship = "Owner", UserId = users[0].Id });
                await em_context.UserHorses.AddAsync(new UserHorse { Horse = brisa, Relationship = "Owner", UserId = users[1].Id });
                await em_context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error seeding Horses - ", e);
        }
    }
}
