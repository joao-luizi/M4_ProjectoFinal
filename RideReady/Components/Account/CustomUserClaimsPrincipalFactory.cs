using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using RepositoryLibrary.IServices;
using SharedLibrary;
using System.Security.Claims;

namespace RideReady.Components.Account
{
    public class CustomUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<EMUser, IdentityRole>
    {
        private ISchoolService _schoolService;

        public CustomUserClaimsPrincipalFactory(
            UserManager<EMUser> userManager,
            RoleManager<IdentityRole> roleManager,
            IOptions<IdentityOptions> optionsAccessor,
            ISchoolService schoolService)
            : base(userManager, roleManager, optionsAccessor)
        {
            _schoolService = schoolService;
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(EMUser user)
        {
            var identity = await base.GenerateClaimsAsync(user);

            if (!string.IsNullOrEmpty(user.FirstName))
            {
                identity.AddClaim(new Claim("FirstName", user.FirstName));
            }

            if (!string.IsNullOrEmpty(user.LastName))
            {
                identity.AddClaim(new Claim("LastName", user.LastName));
            }

            var schools = await _schoolService.GetUserSchoolsAsync(user.Id);
            var school = schools.FirstOrDefault();

            if(school is not null)
            {
                identity.AddClaim(new Claim("SchoolId", school.SchoolId.ToString()));
            }

            return identity;
        }
    }
}
