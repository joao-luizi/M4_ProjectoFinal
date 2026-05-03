using Microsoft.AspNetCore.Identity;
using RideReady.Data;
using SharedLibrary;

namespace RideReady.Components.Account
{
    internal sealed class IdentityUserAccessor(UserManager<EMUser> userManager, IdentityRedirectManager redirectManager)
    {
        public async Task<EMUser> GetRequiredUserAsync(HttpContext context)
        {
            var user = await userManager.GetUserAsync(context.User);

            if (user is null)
            {
                redirectManager.RedirectToWithStatus("Account/InvalidUser", $"Error: Unable to load user with ID '{userManager.GetUserId(context.User)}'.", context);
            }

            return user;
        }
    }
}
