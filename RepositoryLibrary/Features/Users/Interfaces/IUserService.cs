using RepositoryLibrary.Features.Horses.DTOs;
using RepositoryLibrary.Features.Users.DTOs;

namespace RepositoryLibrary.Features.Users.Interfaces
{
    public interface IUserService
    {
        //V2 Implemented
        Task<AdminUsersDto> GetAdminUsersAsync();
        //V2 Implemented
        Task DeactivateUserAsync(string userId);
        //V2 Implemented
        Task ActivateUserAsync(string userId);

        //V2 Implemented
        Task<AdminUserDetailsDto?> GetUserDetailsAsync(string userId);
        
 

        //V2 Implemented
        Task SaveUserAsync(AdminUserDetailsDto dto);

    }
}
