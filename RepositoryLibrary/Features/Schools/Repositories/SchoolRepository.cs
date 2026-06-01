using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Schools.Entities;

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

        public async Task<Logo> AddSchoolLogoAsync(int schoolId, string logoName, string filepath)
        {
            _logger.LogInformation("BD: a adicionar logótipo '{LogoName}' à escola {SchoolId}.", logoName, schoolId);
            try
            {
                Logo? logo = await _emContext.Logos.FirstOrDefaultAsync(lg => lg.SchoolId == schoolId && lg.LogoName == logoName);
                School? school = await _emContext.Schools.FirstOrDefaultAsync(scl => scl.SchoolId == schoolId);

                if (logo is not null)
                {
                    _logger.LogWarning("Logótipo '{LogoName}' já existe para a escola {SchoolId}.", logoName, schoolId);
                    throw new Exception($"The logo with the name {logoName} already exists.");
                }

                if (school is null)
                {
                    _logger.LogWarning("Não foi possível adicionar logótipo: escola {SchoolId} não existe.", schoolId);
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

                _logger.LogInformation("BD: logótipo '{LogoName}' adicionado à escola {SchoolId} ({Size} bytes).", logoName, schoolId, logoImage.Length);
                return newLogo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao adicionar logótipo '{LogoName}' à escola {SchoolId}.", logoName, schoolId);
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


        public async Task<Logo> UpdateSchoolLogoAsync(Logo logoToChange, string filepath)
        {
            _logger.LogInformation("BD: a atualizar logótipo '{LogoName}' da escola {SchoolId}.", logoToChange.LogoName, logoToChange.SchoolId);
            try
            {
                Logo? logo = await _emContext.Logos.FirstOrDefaultAsync(lg => lg.SchoolId == logoToChange.SchoolId && lg.LogoName == logoToChange.LogoName);

                if (logo is null)
                {
                    _logger.LogWarning("Logótipo '{LogoName}' não encontrado para a escola {SchoolId}.", logoToChange.LogoName, logoToChange.SchoolId);
                    throw new Exception($"The logo with the name {logoToChange.LogoName} and Id = {logoToChange.SchoolId} does not exist.");
                }

                byte[] logoImage = await File.ReadAllBytesAsync(filepath);

                logoToChange.Image = logoImage;

                _emContext.Logos.Update(logoToChange);
                _emContext.SaveChanges();

                _logger.LogInformation("BD: logótipo '{LogoName}' da escola {SchoolId} atualizado ({Size} bytes).", logoToChange.LogoName, logoToChange.SchoolId, logoImage.Length);
                return logoToChange;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao atualizar logótipo '{LogoName}' da escola {SchoolId}.", logoToChange.LogoName, logoToChange.SchoolId);
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


        public async Task<Logo> DeleteSchoolLogoAsync(int schoolId, string logoName)
        {
            _logger.LogInformation("BD: a eliminar logótipo '{LogoName}' da escola {SchoolId}.", logoName, schoolId);
            try
            {
                Logo? logo = await _emContext.Logos.FirstOrDefaultAsync(lg => lg.SchoolId == schoolId && lg.LogoName == logoName);

                if (logo is null)
                {
                    _logger.LogWarning("Logótipo '{LogoName}' não encontrado para a escola {SchoolId}.", logoName, schoolId);
                    throw new Exception($"The logo with the name {logoName} and Id = {schoolId} does not exist.");
                }

                _emContext.Logos.Remove(logo);
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
                return await _emContext.Schools.FirstOrDefaultAsync(s => s.SchoolId == schoolId) ?? throw new Exception("School not found.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao consultar escola {SchoolId}.", schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Logo> GetSchoolLogoAsync(int schoolId, string logoName)
        {
            _logger.LogInformation("BD: a consultar logótipo '{LogoName}' da escola {SchoolId}.", logoName, schoolId);
            try
            {
                Logo? logo = await _emContext.Logos.FirstOrDefaultAsync(lg => lg.SchoolId == schoolId && lg.LogoName == logoName);

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

        public async Task<List<Logo>> GetAllSchoolLogosAsync(int schoolId)
        {
            _logger.LogInformation("BD: a consultar logótipos da escola {SchoolId}.", schoolId);
            try
            {
                List<Logo> logos = await _emContext.Logos.Where(scl => scl.SchoolId == schoolId).ToListAsync();

                if (logos.IsNullOrEmpty())
                {
                    _logger.LogWarning("Escola {SchoolId} não tem logótipos.", schoolId);
                    throw new Exception($"The school with Id = {schoolId} has no logos yet.");
                }

                _logger.LogInformation("BD: obtidos {Count} logótipos para a escola {SchoolId}.", logos.Count, schoolId);
                return logos;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao consultar logótipos da escola {SchoolId}.", schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
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