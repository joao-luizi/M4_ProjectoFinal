using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;

namespace RepositoryLibrary.Features.Users.Interfaces
{
    public interface IUserRepository
    {
        //V2 Implemented
        Task<List<UserAdminProjectionDto>> GetUsersForAdministrationAsync();
        //V2 Implemented
        Task<IList<string>> GetUserRolesAsync(string userId);
        //V2 Implemented
        Task<List<EMUser>> GetAllUsersAsync();
        //V2 Implemented
        public Task<List<EMUser>> GetUsersByIdsAsync(List<string> ids);
        //V2 Implemented
        public Task<List<EMUser>> GetUsersByRoleAsync(string role);
        //V2 Implemented
        Task SetUserActiveStatusAsync(string userId, bool isActive);

        //V2 Implemented
        Task UpdateUserAsync(EMUser user);

        //V2 Implemented
        Task<EMUser?> GetByIdAsync(string id);
        

    }
}
