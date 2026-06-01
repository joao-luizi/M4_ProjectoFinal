using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Bookings.Repositories;
using RepositoryLibrary.Features.Horses.Entities;
using RepositoryLibrary.Features.Lessons.Repositories;
using RepositoryLibrary.Features.Schools.Repositories;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using RepositoryLibrary.Features.Users.Repository;
using RepositoryLibrary.IRepository;
using RepositoryLibrary.Repository;

namespace RepositoryLibrary.Features.Users.Service
{
    public class UserService : IUserService
    {
        private readonly UserRepository _userRepository;

        private readonly BookingRepository _bookingRepository;
        private readonly LessonRepository _lessonRepository;
        private readonly HorseRepository _horseRepository;
        private readonly SchoolUsersRepository _schoolUserRepository;
        private readonly UserPhotoRepository _userPhotoRepository;
        
        public UserService(RideReadyDbContext emContext, UserManager<EMUser> userManager)
        {
            _userRepository = new UserRepository(emContext, userManager);

            _bookingRepository = new BookingRepository(emContext);
            _lessonRepository = new LessonRepository(emContext);
            _horseRepository = new HorseRepository(emContext);
            _schoolUserRepository = new SchoolUsersRepository(emContext);
            _userPhotoRepository = new UserPhotoRepository(emContext);
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

        public async Task<List<UpdateUserDto>> GetUsersBySchoolAndRole(int schoolId, string role)
        {
            try
            {
                var users = await _userRepository.GetUsersByRole(role);
                var schoolUsers = await _schoolUserRepository.GetAllUsers(schoolId);

                var schoolUserIds = schoolUsers
                .Select(su => su.UserId)
                .ToHashSet();

               return users.Where(u => schoolUserIds.Contains(u.Id)).ToList();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task DeleteUserAsync(string id)
        {
            try
            {


                //não apagar destrutivamente
                //int deletedHorses = await _horseRepository.DeleteByUserIdAsync(id);
                //int deletedUserPhotos = await _userPhotoRepository.DeletePhotoByUserIdAsync(id);
                //int deletedSchoolUsers = await _schoolUserRepository.DeleteUserAsync(id);

                // 2. inativar user

               await _userRepository.InactivateUser(id);
               
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e);
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
