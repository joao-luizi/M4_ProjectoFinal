using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Schools.Entities;
using RepositoryLibrary.Features.Schools.Repositories;
using RepositoryLibrary.Features.Users;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Repository;
using RepositoryLibrary.IServices;

namespace RepositoryLibrary.Services
{
    public class ImageService : IImageService
    {
        private readonly UserRepository _userRepository;
        private readonly SchoolRepository _schoolRepository;
        private readonly ILogger<ImageService> _logger;

        public ImageService(
            RideReadyDbContext emContext,
            UserManager<EMUser> userManager,
            ILogger<ImageService> logger,
            ILogger<UserRepository> userRepoLogger,
            ILogger<SchoolRepository> schoolRepoLogger)
        {
            _userRepository = new UserRepository(emContext, userManager, userRepoLogger);
            _schoolRepository = new SchoolRepository(emContext, schoolRepoLogger);
            _logger = logger;
        }

        public async Task<Photo> AddUserPhotoAsync(string userId, string filepath)
        {
            _logger.LogInformation("ImageService: a adicionar foto ao utilizador {UserId}.", userId);
            try
            {
                return await _userRepository.AddUserPhotoAsync(userId, filepath);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro no ImageService ao adicionar foto ao utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Photo> UpdateUserPhotoAsync(string userId, string filepath)
        {
            _logger.LogInformation("ImageService: a atualizar foto do utilizador {UserId}.", userId);
            try
            {
                return await _userRepository.UpdateUserPhotoAsync(userId, filepath);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro no ImageService ao atualizar foto do utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Photo> DeleteUserPhotoAsync(string userId)
        {
            _logger.LogInformation("ImageService: a eliminar foto do utilizador {UserId}.", userId);
            try
            {
                return await _userRepository.DeleteUserPhotoAsync(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro no ImageService ao eliminar foto do utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Photo> GetUserPhotoAsync(string userId)
        {
            _logger.LogInformation("ImageService: a obter foto do utilizador {UserId}.", userId);
            try
            {
                return await _userRepository.GetUserPhotoAsync(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro no ImageService ao obter foto do utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Logo> AddSchoolLogoAsync(int schoolId, string logoName, string filepath)
        {
            _logger.LogInformation("ImageService: a adicionar logótipo '{LogoName}' à escola {SchoolId}.", logoName, schoolId);
            try
            {
                return await _schoolRepository.AddSchoolLogoAsync(schoolId, logoName, filepath);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro no ImageService ao adicionar logótipo '{LogoName}' à escola {SchoolId}.", logoName, schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Logo> UpdateSchoolLogoAsync(Logo logoToChange, string filepath)
        {
            _logger.LogInformation("ImageService: a atualizar logótipo '{LogoName}' da escola {SchoolId}.", logoToChange.LogoName, logoToChange.SchoolId);
            try
            {
                return await _schoolRepository.UpdateSchoolLogoAsync(logoToChange, filepath);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro no ImageService ao atualizar logótipo '{LogoName}' da escola {SchoolId}.", logoToChange.LogoName, logoToChange.SchoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Logo> DeleteSchoolLogoAsync(int schoolId, string logoName)
        {
            _logger.LogInformation("ImageService: a eliminar logótipo '{LogoName}' da escola {SchoolId}.", logoName, schoolId);
            try
            {
                return await _schoolRepository.DeleteSchoolLogoAsync(schoolId, logoName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro no ImageService ao eliminar logótipo '{LogoName}' da escola {SchoolId}.", logoName, schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Logo> GetSchoolLogoAsync(int schoolId, string logoName)
        {
            _logger.LogInformation("ImageService: a obter logótipo '{LogoName}' da escola {SchoolId}.", logoName, schoolId);
            try
            {
                return await _schoolRepository.GetSchoolLogoAsync(schoolId, logoName);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro no ImageService ao obter logótipo '{LogoName}' da escola {SchoolId}.", logoName, schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<Logo>> GetAllSchoolLogosAsync(int schoolId)
        {
            _logger.LogInformation("ImageService: a obter logótipos da escola {SchoolId}.", schoolId);
            try
            {
                return await _schoolRepository.GetAllSchoolLogosAsync(schoolId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro no ImageService ao obter logótipos da escola {SchoolId}.", schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}