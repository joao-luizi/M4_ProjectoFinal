using RepositoryLibrary.Models;
using RepositoryLibrary.Models.DTOs;

namespace RepositoryLibrary.IRepository
{
    public interface IUserRepository
    {
        public Task<List<UpdateUserDto>> GetAllUsers(int schoolId);
        public Task<UserDTO> GetUserById(string id);
        public Task<List<UpdateUserDto>> GetUsersByRole(string role);
        public Task<UpdateUserDto> GetEditUserAsync(string id);
        public Task<UserDTO> DeleteUserAsync(string id);
        public Task<UpdateUserDto> EditUserAsync(UpdateUserDto user);
        public Task<Photo> AddUserPhotoAsync(string userId, string filepath);
        public Task<Photo> UpdateUserPhotoAsync(string userId, string filepath);
        public Task<Photo> DeleteUserPhotoAsync(string userId);
        public Task<Photo> GetUserPhotoAsync(string userId);
    }
}
