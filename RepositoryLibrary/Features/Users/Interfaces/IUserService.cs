using RepositoryLibrary.Features.Horses.DTOs;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;

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
        Task<List<EMUser>> GetUsersBySchoolAndRole(int schoolId, string role);

        //V2 Implemented 
        Task SaveUserAsync(AdminUserDetailsDto dto);
        //V2 Implemented 
        Task<List<AdminUserListItemDto>> GetUserListItemsAsync();

    }
}
