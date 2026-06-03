using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Data.Seeds
{
    public static class SeedData
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            await RoleSeed.SeedRolesAsync(services);
            await UserSeed.UserSeedWithRole(services);
            await SchoolSeed.SeedSchoolAsync(services);
            await SchoolSeed.SeedSchoolUserAsync(services);
            await LessonTypeSeed.SeedLessonTypeAsync(services);
            await LessonSeed.SeedLessons(services);
            await HorseSeed.SeedHorses(services);
            await ProductSeed.SeedProductsAsync(services);
            await EntitlementSeed.SeedEntitlementsAsync(services);

        }
    }
}
