using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;

namespace RepositoryLibrary.Features.Users.Interfaces
{
    public interface IUserRepository
    {


        public Task<List<UpdateUserDto>> GetUsersBySchoolAndRole(int schoolId, string role);
        public Task<List<UpdateUserDto>> GetAllUsers(int schoolId);
        public Task<UserDTO> GetUserById(string id);
        public Task<List<UpdateUserDto>> GetUsersByRole(string role);
        public Task<UpdateUserDto> GetEditUserAsync(string id);
        public Task InactivateUser(string id);
        public Task<UpdateUserDto> EditUserAsync(UpdateUserDto user);
        public Task<UserPhoto> AddUserPhotoAsync(string userId, string filepath);
        public Task<UserPhoto> UpdateUserPhotoAsync(string userId, string filepath);
        public Task<UserPhoto> DeleteUserPhotoAsync(string userId);
        public Task<UserPhoto> GetUserPhotoAsync(string userId);
    }
}
