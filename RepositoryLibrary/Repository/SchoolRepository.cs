
using Microsoft.IdentityModel.Tokens;
using RepositoryLibrary.IRepository;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLibrary.Repository
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly EM_DbContext _emContext;

        public SchoolRepository(EM_DbContext context)
        {
            _emContext = context;
        }

        public async Task CreateUserSchoolAsync(string userId, int schoolId)
        {
            try
            {
                var school = await GetSchoolAsync(schoolId);

                if (school is null)
                {
                    throw new Exception("Couldn't find the specified school.");
                }

                //verification to delete, because one user can be in many schools
                var exists = _emContext.SchoolUsers.Any(sc => sc.UserId == userId);
                if (exists)
                {
                    throw new Exception("The user already exists.");
                }

                SchoolUser schoolUser = new SchoolUser
                {
                    UserId = userId,
                    SchoolId = schoolId
                };

                await _emContext.SchoolUsers.AddAsync(schoolUser);
                await _emContext.SaveChangesAsync();
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
                Logo? logo = await _emContext.Logos.FirstOrDefaultAsync(lg => lg.SchoolId == schoolId && lg.LogoName == logoName);
                School? school = await _emContext.Schools.FirstOrDefaultAsync(scl => scl.SchoolId == schoolId);

                if (logo is not null)
                {
                    throw new Exception($"The logo with the name {logoName} already exists.");
                }

                if (school is null)
                {
                    throw new Exception($"The school with the Id = {schoolId} does not exist.");
                }

                byte[] logoImage = await File.ReadAllBytesAsync(filepath);

                Logo newLogo = new Logo
                {
                    SchoolId = schoolId,
                    LogoName = logoName,
                    Image = logoImage
                };

                await _emContext.Logos.AddAsync(newLogo);
                _emContext.SaveChanges();

                return newLogo;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<School> DeleteSchoolAsync(School school)
        {
            try
            {
                _emContext.Schools.Remove(school);
                await _emContext.SaveChangesAsync();
                return school;
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
                Logo? logo = await _emContext.Logos.FirstOrDefaultAsync(lg => lg.SchoolId == logoToChange.SchoolId && lg.LogoName == logoToChange.LogoName);

                if (logo is null)
                {
                    throw new Exception($"The logo with the name {logoToChange.LogoName} and Id = {logoToChange.SchoolId} does not exist.");
                }

                byte[] logoImage = await File.ReadAllBytesAsync(filepath);

                logoToChange.Image = logoImage;

                _emContext.Logos.Update(logoToChange);
                _emContext.SaveChanges();

                return logoToChange;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<School> EditSchoolAsync(School school)
        {
            try
            {
                _emContext.Update(school);
                await _emContext.SaveChangesAsync();
                return school;

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
                Logo? logo = await _emContext.Logos.FirstOrDefaultAsync(lg => lg.SchoolId == schoolId && lg.LogoName == logoName);

                if (logo is null)
                {
                    throw new Exception($"The logo with the name {logoName} and Id = {schoolId} does not exist.");
                }

                _emContext.Logos.Remove(logo);
                _emContext.SaveChanges();

                return logo;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<School> GetSchoolAsync(int schoolId)
        {
            try
            {
                return await _emContext.Schools.FirstOrDefaultAsync(s => s.SchoolId == schoolId) ?? throw new Exception("School not found.");

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Logo> GetSchoolLogoAsync(int schoolId, string logoName)
        {
            try
            {
                Logo? logo = await _emContext.Logos.FirstOrDefaultAsync(lg => lg.SchoolId == schoolId && lg.LogoName == logoName);

                if (logo is null)
                {
                    throw new Exception($"The logo with the name {logoName} and Id = {schoolId} does not exist.");
                }

                return logo;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<School>> GetSchoolsAsync()
        {
            try
            {
                return await _emContext.Schools.ToListAsync();

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
                List<Logo> logos = await _emContext.Logos.Where(scl => scl.SchoolId == schoolId).ToListAsync();

                if (logos.IsNullOrEmpty())
                {
                    throw new Exception($"The school with Id = {schoolId} has no logos yet.");
                }

                return logos;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<School>> GetUserSchoolsAsync(string userId)
        {
            if (userId == null)
                throw new ArgumentNullException(nameof(userId), "userId was null");
            if (_emContext == null)
                throw new InvalidOperationException("_emContext is not initialized");
            if (_emContext.SchoolUsers == null)
                throw new InvalidOperationException("DbSet<SchoolUser> SchoolUsers is missing on the context");

            try
            {
                return await _emContext.SchoolUsers
                    .Where(su => su.UserId == userId)
                    .Include(su => su.School)
                    .Select(su => su.School)
                    .ToListAsync();
            }
            catch (Exception e)
            {
                // Log e (including stack trace) before wrapping
                throw new Exception("Failed loading user schools", e);
            }
        }
    }
}
