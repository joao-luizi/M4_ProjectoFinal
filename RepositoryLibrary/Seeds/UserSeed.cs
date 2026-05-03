using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary;
using Microsoft.Extensions.DependencyInjection;
using SharedLibrary.Models.Static_Class;

namespace EquestrianManagement.Seeds
{
    public static class UserSeed
    {
        public static async Task<List<EMUser>> CreateUsers(IServiceProvider serviceProvider)
        {
            try
            {
                var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var _userManager = serviceProvider.GetRequiredService<UserManager<EMUser>>();

                List<EMUser> eMUsers = new List<EMUser>();

                var superAdmin = new EMUser
                {
                    FirstName = "Super",
                    LastName = "Admin",
                    UserName = "admin@admin.pt",
                    Email = "admin@admin.pt",
                    EmailConfirmed = true,
                    Address = "Rua dos Admins",
                    Birthdate = DateTime.Now,
                    RegisterDate = DateTime.Now,
                    ImageAuthorized = true,
                    InformationAuthorized = true,
                    PhoneNumber = "999999999",
                    PhoneNumberConfirmed = true,
                    CitizenNumber = 111111111,
                    IsActive = true,
                    SocialHealthNumber = 222222222,
                    TaxIdentificationNumber = 333333333
                };

                var adminResult = await _userManager.CreateAsync(superAdmin, "@SuperAdmin1");

                if (!adminResult.Succeeded)
                {
                    throw new Exception($"There was an error creating {superAdmin.UserName}.");
                }

                eMUsers.Add(superAdmin);

                var teacher = new EMUser
                {
                    FirstName = "João",
                    LastName = "Silva",
                    UserName = "joao.silva@email.pt",
                    Email = "joao.silva@email.pt",
                    EmailConfirmed = true,
                    Address = "Rua das Flores, nº10",
                    Birthdate = new DateTime(1985, 3, 15),
                    RegisterDate = DateTime.Now,
                    ImageAuthorized = true,
                    InformationAuthorized = true,
                    PhoneNumber = "912345678",
                    PhoneNumberConfirmed = true,
                    CitizenNumber = 123456789,
                    IsActive = true,
                    SocialHealthNumber = 987654321,
                    TaxIdentificationNumber = 501234567
                };

                var teacherResult = await _userManager.CreateAsync(teacher, "Joao@2024");

                if (!teacherResult.Succeeded)
                {
                    throw new Exception($"There was an error creating {teacher.UserName}.");
                }

                eMUsers.Add(teacher);

                var student_1 = new EMUser
                {
                    FirstName = "Maria",
                    LastName = "Ferreira",
                    UserName = "maria.ferreira@email.pt",
                    Email = "maria.ferreira@email.pt",
                    EmailConfirmed = true,
                    Address = "Avenida da Liberdade, nº55",
                    Birthdate = new DateTime(1990, 7, 22),
                    RegisterDate = DateTime.Now,
                    ImageAuthorized = true,
                    InformationAuthorized = true,
                    PhoneNumber = "913456789",
                    PhoneNumberConfirmed = true,
                    CitizenNumber = 234567890,
                    IsActive = true,
                    SocialHealthNumber = 876543210,
                    TaxIdentificationNumber = 502345678
                };

                var student_1Result = await _userManager.CreateAsync(student_1, "Maria#89");

                if (!student_1Result.Succeeded)
                {
                    throw new Exception($"There was an error creating {student_1.UserName}.");
                }

                eMUsers.Add(student_1);

                var student_2 = new EMUser
                {
                    FirstName = "Carlos",
                    LastName = "Santos",
                    UserName = "carlos.santos@email.pt",
                    Email = "carlos.santos@email.pt",
                    EmailConfirmed = true,
                    Address = "Travessa do Carmo, nº3",
                    Birthdate = new DateTime(1978, 12, 5),
                    RegisterDate = DateTime.Now,
                    ImageAuthorized = true,
                    InformationAuthorized = true,
                    PhoneNumber = "914567890",
                    PhoneNumberConfirmed = true,
                    CitizenNumber = 345678901,
                    IsActive = true,
                    SocialHealthNumber = 765432109,
                    TaxIdentificationNumber = 503456789
                };

                var student_2Result = await _userManager.CreateAsync(student_2, "Car!os1978");

                if (!student_2Result.Succeeded)
                {
                    throw new Exception($"There was an error creating {student_2.UserName}.");
                }

                eMUsers.Add(student_2);

                var student_3 = new EMUser
                {
                    FirstName = "Ana",
                    LastName = "Costa",
                    UserName = "ana.costa@email.pt",
                    Email = "ana.costa@email.pt",
                    EmailConfirmed = true,
                    Address = "Rua da Esperança, nº69",
                    Birthdate = new DateTime(1995, 9, 30),
                    RegisterDate = DateTime.Now,
                    ImageAuthorized = true,
                    InformationAuthorized = true,
                    PhoneNumber = "915678901",
                    PhoneNumberConfirmed = true,
                    CitizenNumber = 456789012,
                    IsActive = true,
                    SocialHealthNumber = 654321098,
                    TaxIdentificationNumber = 504567890
                };

                var student_3Result = await _userManager.CreateAsync(student_3, "AnaC@1995");

                if (!student_3Result.Succeeded)
                {
                    throw new Exception($"There was an error creating {student_3.UserName}.");
                }

                eMUsers.Add(student_3);


                return eMUsers;
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
                var _userManager = serviceProvider.GetRequiredService<UserManager<EMUser>>();
                var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var _emContext = serviceProvider.GetRequiredService<EM_DbContext>();

                bool isCreated = false;

                if (_userManager.Users.Count() <= 0)
                {
                    var usersToDelete = _emContext.SchoolUsers;

                    if (!usersToDelete.IsNullOrEmpty())
                    {
                        foreach (SchoolUser schoolUser in usersToDelete)
                        {
                            _emContext.SchoolUsers.Remove(schoolUser);
                        }

                        await _emContext.SaveChangesAsync();
                    }

                    var users = await CreateUsers(serviceProvider);

                    if (users.IsNullOrEmpty())
                    {
                        throw new Exception("There was an error creating users. On UserSeedWithRole");
                    }

                    var roleExists = await _roleManager.RoleExistsAsync(StaticRole.Admin);

                    if (!roleExists)
                    {
                        throw new Exception("The role does not exist.");
                    }

                    foreach (EMUser user in users)
                    {
                        var result = false;
                        if (user.UserName == "admin@admin.pt")
                        {
                            var adminResult = await _userManager.AddToRoleAsync(user, StaticRole.Admin);
                            result = adminResult.Succeeded;
                        }

                        if (user.UserName == "joao.silva@email.pt")
                        {
                            var teacherResult = await _userManager.AddToRoleAsync(user, StaticRole.Teacher);
                            result = teacherResult.Succeeded;
                        }

                        if (user.UserName != "joao.silva@email.pt" && user.UserName != "admin@admin.pt")
                        {
                            var studentResult = await _userManager.AddToRoleAsync(user, StaticRole.Student);
                            result = studentResult.Succeeded;
                        }

                        if (!result)
                        {
                            throw new Exception($"There was an error when giving a role to {user.UserName}");
                        }

                        isCreated = result;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

    }
}
