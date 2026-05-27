using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Features.Users.Entities;

namespace RideReady.Data
{
    public class AppIdentityDbContext : IdentityDbContext<EMUser>
    {
        public AppIdentityDbContext(DbContextOptions<AppIdentityDbContext> options)
            : base(options)
        {
        }
    }
}
