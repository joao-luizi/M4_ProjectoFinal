using Microsoft.Extensions.Logging;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Users.Services
{
    public class SchoolUserService : ISchoolUserService
    {
        private readonly ISchoolUsersRepository _schoolUserRepository;
        private readonly IUserRepository _userRepository;
        private readonly ISchoolRepository _schoolRepository;
        private readonly ILogger<SchoolUserService> _logger;

        public SchoolUserService(
            ISchoolUsersRepository schoolUserRepository,
            IUserRepository userRepository,
            ISchoolRepository schoolRepository,
            ILogger<SchoolUserService> logger)
        {
            _schoolUserRepository = schoolUserRepository;
            _userRepository = userRepository;
            _schoolRepository = schoolRepository;
            _logger = logger;
        }

        public async Task AddUserToSchoolAsync(string userId, int schoolId)
        {
            _logger.LogInformation("A adicionar user {UserId} à escola {SchoolId}", userId, schoolId);

            try
            {
                var user = await _userRepository.GetByIdAsync(userId);
                if (user == null)
                    throw new Exception($"User {userId} não encontrado");

                var school = await _schoolRepository.GetSchoolAsync(schoolId);
                if (school == null)
                    throw new Exception($"School {schoolId} não encontrada");

                var exists = await _schoolUserRepository.ExistsAsync(userId, schoolId);
                if (exists)
                {
                    _logger.LogWarning("User {UserId} já pertence à escola {SchoolId}", userId, schoolId);
                    return;
                }

                await _schoolUserRepository.AddAsync(new SchoolUser
                {
                    UserId = userId,
                    SchoolId = schoolId
                });

                _logger.LogInformation("User {UserId} adicionado à escola {SchoolId}", userId, schoolId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao adicionar user {UserId} à escola {SchoolId}", userId, schoolId);
                throw;
            }
        }

        public async Task RemoveUserFromSchoolAsync(string userId, int schoolId)
        {
            _logger.LogInformation("A remover user {UserId} da escola {SchoolId}", userId, schoolId);

            try
            {
                var relation = await _schoolUserRepository.GetAsync(userId, schoolId);

                if (relation == null)
                {
                    _logger.LogWarning("Relação User-Escola não existe");
                    return;
                }

                await _schoolUserRepository.DeleteAsync(relation);

                _logger.LogInformation("User {UserId} removido da escola {SchoolId}", userId, schoolId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao remover user {UserId} da escola {SchoolId}", userId, schoolId);
                throw;
            }
        }

       

        public async Task<List<UserSchoolDto>> GetUserSchoolsAsync(string userId)
        {
            _logger.LogInformation("A obter escolas do user {UserId}", userId);

            try
            {
                var schools = await _schoolUserRepository.GetSchoolsByUserAsync(userId);

                return schools.Select(s => new UserSchoolDto
                {
                    SchoolId = s.SchoolId,
                    SchoolName = s.SchoolName
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter escolas do user {UserId}", userId);
                throw;
            }
        }
    }
}
