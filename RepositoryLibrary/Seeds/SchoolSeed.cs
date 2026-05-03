using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Hosting;
using SharedLibrary;
using Microsoft.IdentityModel.Tokens;

namespace EquestrianManagement.Seeds;

public static class SchoolSeed
{
    public static async Task SeedSchoolAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var em_context = serviceProvider.GetRequiredService<EM_DbContext>();
            if (!em_context.Schools.Any())
            {
                List<School> schools =
                [
                // new School{SchoolName = "Test", Address = "Test Address", CAE = 912830, Contact = "216758478", Email = "School1@email.com", SchoolCapacity = 25},
                // new School{SchoolName = "Test2", Address = "Test Address2", CAE = 91283023, Contact = "2167584782", Email = "School2@email.com", SchoolCapacity = 25},
                new School {
                SchoolName = "Centro Equestre do Tejo",
                Address = "Avenida das Cavalgadas, Lisboa",
                CAE = 123456,
                Contact = "912345678",
                Email = "info@centroequestredotejo.pt",
                SchoolCapacity = 30
                },
                new School {
                SchoolName = "Quinta Equestre do Douro",
                Address = "Rua dos Cavalos, Porto",
                CAE = 234567,
                Contact = "923456789",
                Email = "contato@quintaequestredodouro.pt",
                SchoolCapacity = 30
                }
            ];
                await em_context.AddRangeAsync(schools);
                await em_context.SaveChangesAsync();
                var userManager = serviceProvider.GetRequiredService<UserManager<EMUser>>();
                var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var _env = serviceProvider.GetRequiredService<IWebHostEnvironment>();
                var imageService = new ImageService(em_context, userManager);
                var school = await em_context.Schools.FirstOrDefaultAsync(s => s.SchoolName == "Centro Equestre do Tejo") ?? throw new Exception("Couldn't find seeded school.");
                string absolutePath = Path.Combine(_env.WebRootPath, "Logos", "logo1.jpg");
                await imageService.AddSchoolLogoAsync(school.SchoolId, "logo", absolutePath);
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error seeding schools.", e);
        }
    }

    public static async Task SeedSchoolUserAsync(IServiceProvider serviceProvider)
    {
        try
        {
            var em_context = serviceProvider.GetRequiredService<EM_DbContext>();
            var userManager = serviceProvider.GetRequiredService<UserManager<EMUser>>();

            if (em_context.Schools.Any() && !em_context.SchoolUsers.Any())
            {
                // Uncomment if you want to delete all rows from SchoolUsers
                //var rowsAffected = await em_context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE SchoolUsers");
                var users = await userManager.Users.ToListAsync();
                List<SchoolUser> schools = new();

                if (users.IsNullOrEmpty())
                {
                    throw new Exception("There are no users in the DB.");
                }

                foreach(var user in users)
                {
                    SchoolUser schoolUser = new SchoolUser
                    {
                        SchoolId = 1,
                        UserId = user.Id
                    };

                    schools.Add(schoolUser);
                }

                await em_context.SchoolUsers.AddRangeAsync(schools);
                await em_context.SaveChangesAsync();
            }
        }
        catch (Exception e)
        {
            throw new Exception("Error seeding schools.", e);
        }
    }

}
