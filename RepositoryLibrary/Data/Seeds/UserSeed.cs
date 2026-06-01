
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Users.Entities;

namespace RepositoryLibrary.Data.Seeds
{
    public static class UserSeed
    {

        public static async Task<List<EMUser>> CreateUsers(
                            IServiceProvider serviceProvider,
                            int startIndex,
                            int amount,
                            string role)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<EMUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            List<EMUser> users = new();

            var roleExists = await roleManager.RoleExistsAsync(role);

            if (!roleExists)
            {
                throw new Exception($"Role {role} does not exist.");
            }

            for (int i = startIndex; i < startIndex + amount; i++)
            {
                string roleLower = role.ToLower();

                var user = new EMUser
                {
                    FirstName = "Demo",
                    LastName = role,
                    UserName = $"{roleLower}.demo{i}@rideready.com",
                    Email = $"{roleLower}.demo{i}@rideready.com",
                    EmailConfirmed = true,
                    Address = "RideReady HQ",
                    Birthdate = new DateTime(1980, 1, 1),
                    RegisterDate = DateTime.Now,
                    ImageAuthorized = true,
                    InformationAuthorized = true,
                    PhoneNumber = $"9100000{i:D2}",
                    PhoneNumberConfirmed = true,
                    CitizenNumber = 100000000 + i,
                    IsActive = true,
                    SocialHealthNumber = 200000000 + i,
                    TaxIdentificationNumber = 300000000 + i
                };

                string password = $"{role}#demo{i}";

                var result = await userManager.CreateAsync(user, password);

                if (!result.Succeeded)
                {
                    throw new Exception(
                        $"Error creating user {user.UserName}: " +
                        string.Join(", ", result.Errors.Select(e => e.Description)));
                }

                await userManager.AddToRoleAsync(user, role);

                users.Add(user);
            }

            return users;
        }

        public static async Task<List<EMUser>> CreateUsersV0(IServiceProvider serviceProvider)
        {
            try
            {
               

                List<EMUser> users = new();
                List<EMUser> admin = await CreateUsers(serviceProvider, 1, 1, StaticRole.Admin);
                List<EMUser> teachers = await CreateUsers(serviceProvider, 1, 3, StaticRole.Teacher);
                List<EMUser> students = await CreateUsers(serviceProvider, 1, 10, StaticRole.Student);
                users.AddRange(admin);
                users.AddRange(teachers);
                users.AddRange(students);


                return users;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }


        public static async Task UserSeedWithRole(IServiceProvider serviceProvider)
        {
            try
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<EMUser>>();
                var emContext = serviceProvider.GetRequiredService<RideReadyDbContext>();

                if (userManager.Users.Any())
                {
                    return;
                }

                if (emContext.SchoolUsers.Any())
                {
                    emContext.SchoolUsers.RemoveRange(emContext.SchoolUsers);
                    await emContext.SaveChangesAsync();
                }

                List<EMUser> users = new();

                users.AddRange(await CreateUsers(
                    serviceProvider,
                    startIndex: 1,
                    amount: 1,
                    role: StaticRole.Admin));

                users.AddRange(await CreateUsers(
                    serviceProvider,
                    startIndex: 1,
                    amount: 3,
                    role: StaticRole.Teacher));

                users.AddRange(await CreateUsers(
                    serviceProvider,
                    startIndex: 1,
                    amount: 10,
                    role: StaticRole.Student));

                if (!users.Any())
                {
                    throw new Exception("No users were created.");
                }
            }
            catch (Exception e)
            {
                throw new Exception("Error seeding users.", e);
            }
        }

    }
}
