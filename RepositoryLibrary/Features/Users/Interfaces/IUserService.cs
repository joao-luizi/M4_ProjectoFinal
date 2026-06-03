using RepositoryLibrary.Features.Users.DTOs;

namespace RepositoryLibrary.Features.Users.Interfaces
{
    public interface IUserService
    {
        Task<List<UserListDto>> GetAllUsersAsync(int schoolId);

        Task<List<UserListDto>> GetUsersByRoleAsync(string role);

        Task<List<UserListDto>> GetUsersBySchoolAndRoleAsync(int schoolId, string role);

        Task<UserDetailsDto?> GetUserByIdAsync(string userId);

        Task<UpdateUserDto?> GetEditUserAsync(string userId);

        Task UpdateUserAsync(UpdateUserDto user);

        Task DeactivateUserAsync(string userId);

        Task<IList<string>> GetUserRolesAsync(string userId);

        Task SetUserRolesAsync(string userId, IList<string> roles);

        Task<UserPhotoDto?> GetPhotoAsync(string userId);

        Task SetPhotoAsync(string userId, string photoPath);

        Task DeletePhotoAsync(string userId);
    }
}
