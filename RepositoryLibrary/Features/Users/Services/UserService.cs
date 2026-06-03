using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Bookings.Interfaces;
using RepositoryLibrary.Features.Bookings.Repositories;
using RepositoryLibrary.Features.Horses.Interfaces;
using RepositoryLibrary.Features.Horses.Repositories;
using RepositoryLibrary.Features.Images.Interfaces;
using RepositoryLibrary.Features.Lessons.Interfaces;
using RepositoryLibrary.Features.Lessons.Repositories;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Schools.Repositories;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using RepositoryLibrary.Features.Users.Repository;


namespace RepositoryLibrary.Features.Users.Service
{

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISchoolUsersRepository _schoolUserRepository;
        private readonly IUserPhotoRepository _userPhotoRepository;
        private readonly IBookingRepository _bookingRepository;
        private readonly ILessonRepository _lessonRepository;
        private readonly IHorseRepository _horseRepository;
        private readonly IImageService _imageService;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepository,
            ISchoolUsersRepository schoolUserRepository,
            IUserPhotoRepository userPhotoRepository,
            IBookingRepository bookingRepository,
            ILessonRepository lessonRepository,
            IHorseRepository horseRepository,
            IImageService imageService,
            ILogger<UserService> logger)
        {
            _userRepository = userRepository;
            _schoolUserRepository = schoolUserRepository;
            _userPhotoRepository = userPhotoRepository;
            _bookingRepository = bookingRepository;
            _lessonRepository = lessonRepository;
            _horseRepository = horseRepository;
            _imageService = imageService;
            _logger = logger;
        }

        public async Task<List<UserListDto>> GetAllUsersAsync(int schoolId)
        {
            _logger.LogInformation("A obter users da school {SchoolId}", schoolId);

            try
            {
                return await _userRepository.GetAllUsersBySchoolAsync(schoolId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter users da school {SchoolId}", schoolId);
                throw;
            }
        }

        public async Task<List<UserListDto>> GetUsersByRoleAsync(string role)
        {
            _logger.LogInformation("A obter users com role {Role}", role);

            try
            {
                return await _userRepository.GetUsersByRoleAsync(role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter users por role {Role}", role);
                throw;
            }
        }

        public async Task<List<UserListDto>> GetUsersBySchoolAndRoleAsync(int schoolId, string role)
        {
            _logger.LogInformation("A obter users da school {SchoolId} com role {Role}", schoolId, role);

            try
            {
                return await _userRepository.GetUsersBySchoolAndRole(schoolId, role);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter users school+role");
                throw;
            }
        }

        public async Task<UserDetailsDto?> GetUserByIdAsync(string userId)
        {
            _logger.LogInformation("A obter user {UserId}", userId);

            try
            {
                return await _userRepository.GetUserByIdAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter user {UserId}", userId);
                throw;
            }
        }

        public async Task<UpdateUserDto?> GetEditUserAsync(string userId)
        {
            _logger.LogInformation("A obter user para edição {UserId}", userId);

            try
            {
                return await _userRepository.GetEditUserAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter user para edição {UserId}", userId);
                throw;
            }
        }

        public async Task UpdateUserAsync(UpdateUserDto user)
        {
            _logger.LogInformation("A atualizar user {UserId}", user.Id);

            try
            {
                await _userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar user {UserId}", user.Id);
                throw;
            }
        }

        public async Task DeactivateUserAsync(string userId)
        {
            _logger.LogInformation("A desativar user {UserId}", userId);

            try
            {
                await _userRepository.DeactivateAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao desativar user {UserId}", userId);
                throw;
            }
        }

        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            _logger.LogInformation("A obter roles do user {UserId}", userId);

            try
            {
                return await _userRepository.GetUserRolesAsync(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter roles");
                throw;
            }
        }

        public async Task SetUserRolesAsync(string userId, IList<string> roles)
        {
            _logger.LogInformation("A definir roles do user {UserId}", userId);

            try
            {
                await _userRepository.SetUserRolesAsync(userId, roles);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao definir roles");
                throw;
            }
        }

        public async Task<UserPhotoDto?> GetPhotoAsync(string userId)
        {
            var photo = await _userPhotoRepository.GetByUserIdAsync(userId);

            if (photo == null)
                return null;

            return new UserPhotoDto
            {
                UserId = photo.UserId,
                FotoPath = photo.FotoPath
            };
        }

        public async Task SetPhotoAsync(string userId, IBrowserFile file)
        {
            _logger.LogInformation("A atualizar foto do user {UserId}", userId);

            var photo = await _userPhotoRepository.GetByUserIdAsync(userId);

            string folder = "userphotos";
            string fileName = userId;

            // 1. guardar nova imagem (overwrite lógico via mesmo nome)
            var newPath = await _imageService.SaveImageAsync(file, folder, fileName);

            // 2. apagar antiga depois de nova estar segura
            if (photo != null && !string.IsNullOrEmpty(photo.FotoPath))
            {
                await _imageService.DeleteImageAsync(photo.FotoPath);
            }

            // 3. persistência DB
            if (photo == null)
            {
                await _userPhotoRepository.AddAsync(new UserPhoto
                {
                    UserId = userId,
                    FotoPath = newPath
                });

                return;
            }

            photo.FotoPath = newPath;

            await _userPhotoRepository.SaveChangesAsync();
        }

        public async Task DeletePhotoAsync(string userId)
        {
            _logger.LogInformation("A apagar foto do user {UserId}", userId);

            var photo = await _userPhotoRepository.GetByUserIdAsync(userId);

            if (photo == null)
                return;

            // apagar ficheiro físico primeiro
            if (!string.IsNullOrEmpty(photo.FotoPath))
            {
                await _imageService.DeleteImageAsync(photo.FotoPath);
            }

            await _userPhotoRepository.DeleteAsync(photo);
        }

    }
        


    }