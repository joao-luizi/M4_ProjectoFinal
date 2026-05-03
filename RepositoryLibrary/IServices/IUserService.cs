using RepositoryLibrary.Models.DTOs;

namespace RepositoryLibrary.IServices
{
    public interface IUserService
    {
        public Task<List<UpdateUserDto>> GetAllUsers(int schoolId);
        public Task<UserDTO> GetUserById(string id);
        public Task<List<UpdateUserDto>> GetUsersByRole(string role);
        public Task<UserDTO> DeleteUserAsync(string id);

        public Task<UpdateUserDto> GetEditUserAsync(string id);

        public Task<UpdateUserDto> EditUserAsync(UpdateUserDto user);

        public Task<IList<string>> GetUserRole(string userId);
    }
}
