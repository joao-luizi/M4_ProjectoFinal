using RepositoryLibrary.IServices;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Repository;
using Microsoft.AspNetCore.Identity;
using SharedLibrary;
using RepositoryLibrary.Models.DTOs;

namespace RepositoryLibrary.Services
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;

        public UserService(EM_DbContext emContext, UserManager<EMUser> userManager)
        {
            _userRepository = new UserRepository(emContext, userManager);
        }

        public async Task<List<UpdateUserDto>> GetAllUsers(int schoolId)
        {
            try
            {
                return await _userRepository.GetAllUsers(schoolId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UserDTO> GetUserById(string id)
        {
            try
            {
                return await _userRepository.GetUserById(id);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<UpdateUserDto>> GetUsersByRole(string role)
        {
            try
            {
                return await _userRepository.GetUsersByRole(role);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UserDTO> DeleteUserAsync(string id)
        {
            try
            {
                return await _userRepository.DeleteUserAsync(id);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UpdateUserDto> GetEditUserAsync(string id)
        {
            try
            {
                var updatedUser = await _userRepository.GetEditUserAsync(id);

                return updatedUser;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UpdateUserDto> EditUserAsync(UpdateUserDto user)
        {
            try
            {
                return await _userRepository.EditUserAsync(user);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<IList<string>> GetUserRole(string userId)
        {
            try
            {
                return await _userRepository.GetUserRole(userId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
