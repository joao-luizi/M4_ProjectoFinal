using Microsoft.AspNetCore.Identity;
using SharedLibrary.Models.Static_Class;
using Microsoft.Extensions.DependencyInjection;

namespace RepositoryLibrary.Seeds
{
    public static class RoleSeed
    {
        public static async Task SeedRolesAsync(IServiceProvider serviceProvider)
        {
            try
            {
                var _roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                bool result = false;
                if (_roleManager.Roles.Count() <= 0)
                {
                    List<IdentityRole> roles = new List<IdentityRole>
                    {
                        new IdentityRole { Name = StaticRole.Admin },
                        new IdentityRole { Name = StaticRole.Teacher },
                        new IdentityRole { Name = StaticRole.Student }
                    };

                    foreach(IdentityRole role in roles)
                    {
                        var creationResult = await _roleManager.CreateAsync(role);
                    
                        if(!creationResult.Succeeded)
                        {
                            throw new Exception($"There was an error when trying to create the Role {role.Name}");
                        }

                        result = creationResult.Succeeded;
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
