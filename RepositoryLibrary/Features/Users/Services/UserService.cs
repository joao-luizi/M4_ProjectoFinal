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
        private readonly ILogger<UserService> _logger;

        public UserService(RideReadyDbContext emContext, UserManager<EMUser> userManager, ILogger<UserService> logger, ILogger<UserRepository> userRepoLogger, ILogger<LessonRepository> lessonRepoLogger)
        {
            _userRepository = new UserRepository(emContext, userManager, userRepoLogger);

            _bookingRepository = new BookingRepository(emContext);
            _lessonRepository = new LessonRepository(emContext, lessonRepoLogger);
            _horseRepository = new HorseRepository(emContext);
            _schoolUserRepository = new SchoolUsersRepository(emContext);
            _userPhotoRepository = new UserPhotoRepository(emContext);
            _logger = logger;
        }

        public async Task<List<UpdateUserDto>> GetAllUsers(int schoolId)
        {
            _logger.LogInformation("A obter todos os utilizadores da escola {SchoolId}.", schoolId);
            try
            {
                var users = await _userRepository.GetAllUsers(schoolId);
                _logger.LogInformation("Obtidos {Count} utilizadores da escola {SchoolId}.", users.Count, schoolId);
                return users;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter os utilizadores da escola {SchoolId}.", schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UserDTO> GetUserById(string id)
        {
            _logger.LogInformation("A obter o utilizador {UserId}.", id);
            try
            {
                var user = await _userRepository.GetUserById(id);
                _logger.LogInformation("Utilizador {UserId} obtido com sucesso.", id);
                return user;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter o utilizador {UserId}.", id);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<UpdateUserDto>> GetUsersByRole(string role)
        {
            _logger.LogInformation("A obter utilizadores com o papel {Role}.", role);
            try
            {
                var users = await _userRepository.GetUsersByRole(role);
                _logger.LogInformation("Obtidos {Count} utilizadores com o papel {Role}.", users.Count, role);
                return users;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter utilizadores com o papel {Role}.", role);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<UpdateUserDto>> GetUsersBySchoolAndRole(int schoolId, string role)
        {
            _logger.LogInformation("A obter utilizadores com o papel {Role} na escola {SchoolId}.", role, schoolId);
            try
            {
                var users = await _userRepository.GetUsersByRole(role);
                var schoolUsers = await _schoolUserRepository.GetAllUsers(schoolId);

                var schoolUserIds = schoolUsers
                .Select(su => su.UserId)
                .ToHashSet();

                var result = users.Where(u => schoolUserIds.Contains(u.Id)).ToList();
                _logger.LogInformation("Obtidos {Count} utilizadores com o papel {Role} na escola {SchoolId}.", result.Count, role, schoolId);
                return result;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter utilizadores com o papel {Role} na escola {SchoolId}.", role, schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task DeleteUserAsync(string id)
        {
            _logger.LogInformation("A inativar o utilizador {UserId}.", id);
            try
            {
                //não apagar destrutivamente
                //int deletedHorses = await _horseRepository.DeleteByUserIdAsync(id);
                //int deletedUserPhotos = await _userPhotoRepository.DeletePhotoByUserIdAsync(id);
                //int deletedSchoolUsers = await _schoolUserRepository.DeleteUserAsync(id);

                // 2. inativar user

                await _userRepository.InactivateUser(id);
                _logger.LogInformation("Utilizador {UserId} inativado com sucesso.", id);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao inativar o utilizador {UserId}.", id);
                throw new Exception(e.Message, e);
            }
        }

        public async Task<UpdateUserDto> GetEditUserAsync(string id)
        {
            _logger.LogInformation("A obter dados de edição do utilizador {UserId}.", id);
            try
            {
                var updatedUser = await _userRepository.GetEditUserAsync(id);
                _logger.LogInformation("Dados de edição do utilizador {UserId} obtidos com sucesso.", id);
                return updatedUser;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter dados de edição do utilizador {UserId}.", id);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UpdateUserDto> EditUserAsync(UpdateUserDto user)
        {
            _logger.LogInformation("A editar o utilizador {UserId}.", user.Id);
            try
            {
                var edited = await _userRepository.EditUserAsync(user);
                _logger.LogInformation("Utilizador {UserId} editado com sucesso.", user.Id);
                return edited;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao editar o utilizador {UserId}.", user.Id);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<IList<string>> GetUserRole(string userId)
        {
            _logger.LogInformation("A obter os papéis do utilizador {UserId}.", userId);
            try
            {
                var roles = await _userRepository.GetUserRole(userId);
                _logger.LogInformation("Obtidos {Count} papéis para o utilizador {UserId}.", roles.Count, userId);
                return roles;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter os papéis do utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}