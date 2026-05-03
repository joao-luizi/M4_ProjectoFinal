using System.Drawing;
using RepositoryLibrary.Models;

namespace RepositoryLibrary.IServices
{
    public interface IImageService
    {
        public Task<Photo> AddUserPhotoAsync(string userId, string filepath);
        public Task<Photo> UpdateUserPhotoAsync(string userId, string filepath);
        public Task<Photo> DeleteUserPhotoAsync(string userId);
        public Task<Photo> GetUserPhotoAsync(string userId);
        public Task<Logo> AddSchoolLogoAsync(int schoolId, string logoName, string filepath);
        public Task<Logo> UpdateSchoolLogoAsync(Logo logoToChange, string filepath);
        public Task<Logo> DeleteSchoolLogoAsync(int schoolId, string logoName);
        public Task<Logo> GetSchoolLogoAsync(int schoolId, string logoName);
        public Task<List<Logo>> GetAllSchoolLogosAsync(int schoolId);
    }
}
