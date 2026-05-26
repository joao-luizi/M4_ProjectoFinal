using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using SharedLibrary;
using SharedLibrary.Models.Static_Class;

namespace RepositoryLibrary.Seeds;

public static class HorseSeed
{

    public static async Task SeedHorses(IServiceProvider serviceProvider)
    {
        try
        {
            var em_context = serviceProvider.GetRequiredService<EM_DbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<EMUser>>();

            if (await em_context.Horses.AnyAsync())
                return;

            var school = await em_context.Schools
                .FirstOrDefaultAsync(s => s.SchoolId == 1);

            if (school == null)
                throw new Exception("School not found.");

            var horses = new List<Horse>
        {
            new Horse
            {
                Name = "Relâmpago",
                Breed = "Mangalarga Marchador",
                DateOfBirth = new DateTime(2020, 1, 1),
                School = school
            },

            new Horse
            {
                Name = "Estrela",
                Breed = "Puro-Sangue Lusitano",
                DateOfBirth = new DateTime(2022, 1, 1),
                School = school
            },

            new Horse
            {
                Name = "Ventania",
                Breed = "Crioulo",
                DateOfBirth = new DateTime(2019, 1, 1),
                School = school
            },

            new Horse
            {
                Name = "Fumaça",
                Breed = "Quarto de Milha",
                DateOfBirth = new DateTime(2021, 1, 1),
                School = school
            },

            new Horse
            {
                Name = "Brisa",
                Breed = "Árabe",
                DateOfBirth = new DateTime(2023, 1, 1),
                School = school
            },

            new Horse
            {
                Name = "Trovão",
                Breed = "Frísio",
                DateOfBirth = new DateTime(2018, 5, 10),
                School = school
            },

            new Horse
            {
                Name = "Aurora",
                Breed = "Andaluz",
                DateOfBirth = new DateTime(2020, 8, 15),
                School = school
            },

            new Horse
            {
                Name = "Safira",
                Breed = "Appaloosa",
                DateOfBirth = new DateTime(2017, 3, 21),
                School = school
            },

            new Horse
            {
                Name = "Cometa",
                Breed = "Hanoveriano",
                DateOfBirth = new DateTime(2021, 11, 5),
                School = school
            },

            new Horse
            {
                Name = "Nébula",
                Breed = "Percheron",
                DateOfBirth = new DateTime(2016, 7, 30),
                School = school
            }
        };

            await em_context.Horses.AddRangeAsync(horses);
            await em_context.SaveChangesAsync();

            // Buscar utilizadores seedados
            var user1 = await userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == $"{StaticRole.Student.ToLower()}.demo1@rideready.com");

            var user2 = await userManager.Users
                .FirstOrDefaultAsync(u => u.UserName == $"{StaticRole.Student.ToLower()}.demo2@rideready.com");

            // Buscar cavalos específicos
            var horse1 = await em_context.Horses
                .FirstOrDefaultAsync(h => h.Name == "Relâmpago");

            var horse2 = await em_context.Horses
                .FirstOrDefaultAsync(h => h.Name == "Estrela");

            if (user1 != null && horse1 != null)
            {
                await em_context.UserHorses.AddAsync(new UserHorse
                {
                    Horse = horse1,
                    Relationship = "Owner",
                    UserId = user1.Id
                });
            }

            if (user2 != null && horse2 != null)
            {
                await em_context.UserHorses.AddAsync(new UserHorse
                {
                    Horse = horse2,
                    Relationship = "Owner",
                    UserId = user2.Id
                });
            }

            await em_context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Error seeding Horses.", e);
        }
    }
}
