using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Schools.Entities;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Users.Entities;

namespace RepositoryLibrary.Features.Schools.Repositories
{
    public class SchoolRepository : ISchoolRepository
    {
        private readonly RideReadyDbContext _emContext;
        private readonly ILogger<SchoolRepository> _logger;

        public SchoolRepository(RideReadyDbContext context, ILogger<SchoolRepository> logger)
        {
            _emContext = context;
            _logger = logger;
        }

        public async Task<School> CreateSchoolAsync(School school)
        {
            if (school == null)
                throw new ArgumentNullException(nameof(school));

            await _emContext.Schools.AddAsync(school);
            await _emContext.SaveChangesAsync();

            return school;
        }
        public async Task CreateUserSchoolAsync(string userId, int schoolId)
        {
            _logger.LogInformation("BD: a associar utilizador {UserId} à escola {SchoolId}.", userId, schoolId);
            try
            {
                var school = await GetSchoolAsync(schoolId);

                if (school is null)
                {
                    _logger.LogWarning("Escola {SchoolId} não encontrada ao associar utilizador.", schoolId);
                    throw new Exception("Couldn't find the specified school.");
                }

                //verification to delete, because one user can be in many schools
                var exists = _emContext.SchoolUsers.Any(sc => sc.UserId == userId);
                if (exists)
                {
                    _logger.LogWarning("Utilizador {UserId} já está associado a uma escola.", userId);
                    throw new Exception("The user already exists.");
                }

                SchoolUser schoolUser = new SchoolUser
                {
                    UserId = userId,
                    SchoolId = schoolId
                };

                await _emContext.SchoolUsers.AddAsync(schoolUser);
                await _emContext.SaveChangesAsync();

                _logger.LogInformation("BD: utilizador {UserId} associado à escola {SchoolId}.", userId, schoolId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao associar utilizador {UserId} à escola {SchoolId}.", userId, schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

       
        public async Task<School> DeleteSchoolAsync(School school)
        {
            _logger.LogInformation("BD: a eliminar escola {SchoolId}.", school.SchoolId);
            try
            {
                _emContext.Schools.Remove(school);
                await _emContext.SaveChangesAsync();
                _logger.LogInformation("BD: escola {SchoolId} eliminada.", school.SchoolId);
                return school;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao eliminar escola {SchoolId}.", school.SchoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }


        public async Task<SchoolPhoto> UpdateSchoolLogoAsync(SchoolPhoto logoToChange, string filepath)
        {
            _logger.LogInformation("BD: a atualizar da escola {SchoolId}.", logoToChange.SchoolId);
            try
            {
                SchoolPhoto? logo = await _emContext.SchoolPhotos.FirstOrDefaultAsync(lg => lg.SchoolId == logoToChange.SchoolId);

                if (logo is null)
                {
                    _logger.LogWarning("Logótipo não encontrado para a escola {SchoolId}.", logoToChange.SchoolId);
                    throw new Exception($"The logo with  Id = {logoToChange.SchoolId} does not exist.");
                }

                byte[] logoImage = await File.ReadAllBytesAsync(filepath);

                

                _emContext.SchoolPhotos.Update(logoToChange);
                _emContext.SaveChanges();

                _logger.LogInformation("BD: logótipo da escola {SchoolId} atualizado ({Size} bytes).", logoToChange.SchoolId);
                return logoToChange;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao atualizar logótipo da escola {SchoolId}.", logoToChange.SchoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<School> EditSchoolAsync(School school)
        {
            _logger.LogInformation("BD: a editar escola {SchoolId}.", school.SchoolId);
            try
            {
                _emContext.Update(school);
                await _emContext.SaveChangesAsync();
                _logger.LogInformation("BD: escola {SchoolId} editada.", school.SchoolId);
                return school;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao editar escola {SchoolId}.", school.SchoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }


        public async Task<SchoolPhoto> DeleteSchoolLogoAsync(int schoolId, string logoName)
        {
            _logger.LogInformation("BD: a eliminar logótipo '{LogoName}' da escola {SchoolId}.", logoName, schoolId);
            try
            {
                SchoolPhoto? logo = await _emContext.SchoolPhotos.FirstOrDefaultAsync(lg => lg.SchoolId == schoolId);

                if (logo is null)
                {
                    _logger.LogWarning("Logótipo '{LogoName}' não encontrado para a escola {SchoolId}.", logoName, schoolId);
                    throw new Exception($"The logo with the name {logoName} and Id = {schoolId} does not exist.");
                }

                _emContext.SchoolPhotos.Remove(logo);
                _emContext.SaveChanges();

                _logger.LogInformation("BD: logótipo '{LogoName}' eliminado da escola {SchoolId}.", logoName, schoolId);
                return logo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao eliminar logótipo '{LogoName}' da escola {SchoolId}.", logoName, schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<School> GetSchoolAsync(int schoolId)
        {
            _logger.LogInformation("BD: a consultar escola {SchoolId}.", schoolId);
            try
            {
                return await _emContext.Schools.Include(x => x.SchoolPhoto)
                    .FirstOrDefaultAsync(s => s.SchoolId == schoolId) ?? throw new Exception("School not found.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao consultar escola {SchoolId}.", schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<SchoolPhoto> GetSchoolLogoAsync(int schoolId, string logoName)
        {
            _logger.LogInformation("BD: a consultar logótipo '{LogoName}' da escola {SchoolId}.", logoName, schoolId);
            try
            {
                SchoolPhoto? logo = await _emContext.SchoolPhotos.FirstOrDefaultAsync(lg => lg.SchoolId == schoolId);

                if (logo is null)
                {
                    _logger.LogWarning("Logótipo '{LogoName}' não encontrado para a escola {SchoolId}.", logoName, schoolId);
                    throw new Exception($"The logo with the name {logoName} and Id = {schoolId} does not exist.");
                }

                return logo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao consultar logótipo '{LogoName}' da escola {SchoolId}.", logoName, schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<School>> GetSchoolsAsync()
        {
            _logger.LogInformation("BD: a consultar todas as escolas.");
            try
            {
                var schools = await _emContext.Schools.ToListAsync();
                _logger.LogInformation("BD: obtidas {Count} escolas.", schools.Count);
                return schools;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao consultar a lista de escolas.");
                throw new Exception(e.Message, e.InnerException);
            }
        }
        //V2 Implemented
        public async Task AddAsync(School school)
        {
            await _emContext.Schools.AddAsync(school);
            await _emContext.SaveChangesAsync();
        }
        //V2 Implemented
        public async Task AddAsync(SchoolPhoto foto)
        {
            await _emContext.SchoolPhotos.AddAsync(foto);
            await _emContext.SaveChangesAsync();
        }
        //V2 Implemented
        public async Task<SchoolPhoto?> GetSchoolPhotoByIsAsync(int schoolId)
        {
            return await _emContext.SchoolPhotos
                .Where(x => x.SchoolId == schoolId)
                .FirstOrDefaultAsync();
        }
        //V2 Implemented
        public async Task UpdateAsync(SchoolPhoto foto)
        {
            _emContext.SchoolPhotos.Update(foto);
            await _emContext.SaveChangesAsync();
        }

        //V2 Implemented
        public async Task UpdateAsync(School school)
        {
            _emContext.Schools.Update(school);
            await _emContext.SaveChangesAsync();
        }
        public async Task<List<School>> GetUserSchoolsAsync(string userId)
        {
            _logger.LogInformation("BD: a consultar escolas do utilizador {UserId}.", userId);

            if (userId == null)
            {
                _logger.LogWarning("Tentativa de consulta com userId nulo.");
                throw new ArgumentNullException(nameof(userId), "userId was null");
            }
            if (_emContext == null)
                throw new InvalidOperationException("_emContext is not initialized");
            if (_emContext.SchoolUsers == null)
                throw new InvalidOperationException("DbSet<SchoolUser> SchoolUsers is missing on the context");

            try
            {
                var schools = await _emContext.SchoolUsers
                    .Where(su => su.UserId == userId)
                    .Include(su => su.School)
                    .Select(su => su.School)
                    .ToListAsync();

                _logger.LogInformation("BD: obtidas {Count} escolas para o utilizador {UserId}.", schools.Count, userId);
                return schools;
            }
            catch (Exception e)
            {
                // Log e (including stack trace) before wrapping
                _logger.LogError(e, "Falha ao consultar escolas do utilizador {UserId}.", userId);
                throw new Exception("Failed loading user schools", e);
            }
        }

       
    }
}