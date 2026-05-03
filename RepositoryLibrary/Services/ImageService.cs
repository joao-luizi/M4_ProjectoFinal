using RepositoryLibrary.IServices;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using RepositoryLibrary.Repository;
using Microsoft.AspNetCore.Identity;
using SharedLibrary;

namespace RepositoryLibrary.Services
{
    public class ImageService : IImageService
    {
        private readonly UserRepository _userRepository;
        private readonly SchoolRepository _schoolRepository;
        public ImageService(EM_DbContext emContext, UserManager<EMUser> userManager)
        {
            _userRepository = new UserRepository(emContext, userManager);
            _schoolRepository = new SchoolRepository(emContext);
        }

        public async Task<Photo> AddUserPhotoAsync(string userId, string filepath)
        {
            try
            {
                return await _userRepository.AddUserPhotoAsync(userId, filepath);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Photo> UpdateUserPhotoAsync(string userId, string filepath)
        {
            try
            {
                return await _userRepository.UpdateUserPhotoAsync(userId, filepath);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Photo> DeleteUserPhotoAsync(string userId)
        {
            try
            {
                return await _userRepository.DeleteUserPhotoAsync(userId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Photo> GetUserPhotoAsync(string userId)
        {
            try
            {
                return await _userRepository.GetUserPhotoAsync(userId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Logo> AddSchoolLogoAsync(int schoolId, string logoName, string filepath)
        {
            try
            {
                return await _schoolRepository.AddSchoolLogoAsync(schoolId, logoName, filepath);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Logo> UpdateSchoolLogoAsync(Logo logoToChange, string filepath)
        {
            try
            {
                return await _schoolRepository.UpdateSchoolLogoAsync(logoToChange, filepath);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
        public async Task<Logo> DeleteSchoolLogoAsync(int schoolId, string logoName)
        {
            try
            {
                return await _schoolRepository.DeleteSchoolLogoAsync(schoolId, logoName);
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Logo> GetSchoolLogoAsync(int schoolId, string logoName)
        {
            try
            {
                return await _schoolRepository.GetSchoolLogoAsync(schoolId, logoName);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<Logo>> GetAllSchoolLogosAsync(int schoolId)
        {
            try
            {
                return await _schoolRepository.GetAllSchoolLogosAsync(schoolId);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
