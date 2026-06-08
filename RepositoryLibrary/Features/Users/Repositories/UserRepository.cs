using DocumentFormat.OpenXml.InkML;
using DocumentFormat.OpenXml.Office2021.Excel.RichDataWebImage;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.IdentityModel.Tokens;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using RideReady.Data;
using System.Data;

namespace RepositoryLibrary.Features.Users.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly RideReadyDbContext _emContext;
        private readonly UserManager<EMUser> _userManager;
        private readonly AppIdentityDbContext _context;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(RideReadyDbContext emContext, AppIdentityDbContext context, UserManager<EMUser> userManager, ILogger<UserRepository>? logger = null)
        {
            _emContext = emContext;
            _userManager = userManager;
            _context = context;
            _logger = logger ?? NullLogger<UserRepository>.Instance;
        }

        //V2 Implemented
        public async Task<List<UserAdminProjectionDto>> GetUsersForAdministrationAsync()
        {
            _logger.LogInformation("BD: a obter utilizadores para administração.");

            var users = await _userManager.Users.ToListAsync();
            var userRoles = await _context.UserRoles.ToListAsync();
            var roles = await _context.Roles.ToListAsync();
            var photos = await _emContext.UserFotos.ToListAsync();

            var roleDict = roles.ToDictionary(r => r.Id, r => r.Name);

            var userRoleDict = userRoles
                .GroupBy(x => x.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault()?.RoleId
                );

            var photoDict = photos
                .GroupBy(p => p.UserId)
                .ToDictionary(
                    g => g.Key,
                    g => g.FirstOrDefault()?.FotoPath
                );

            var result = new List<UserAdminProjectionDto>();

            foreach (var user in users)
            {
                userRoleDict.TryGetValue(user.Id, out var roleId);

                roleDict.TryGetValue(roleId ?? string.Empty, out var roleName);

                photoDict.TryGetValue(user.Id, out var photoPath);

                result.Add(new UserAdminProjectionDto
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email ?? string.Empty,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    Role = roleName,
                    PhotoPath = photoPath
                });
            }

            return result;
        }

        //V2 Implemented
        public async Task<IList<string>> GetUserRolesAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
                return [];

            return await _userManager.GetRolesAsync(user);
        }

        //V2 Implemented
        public async Task<List<EMUser>> GetAllUsersAsync()
        {
            _logger.LogInformation(
                "BD: a obter todos os utilizadores.");

            return await _userManager.Users
                .OrderBy(x => x.FirstName)
                .ThenBy(x => x.LastName)
                .ToListAsync();
        }

        //V2 Implemented
        public async Task<List<EMUser>> GetUsersByIdsAsync(List<string> ids)
        {
            _logger.LogInformation("BD: a obter utilizadores com o id em lista.");
            if (ids == null || ids.Count == 0)
            {
                _logger.LogWarning("Nenhum utilizador encontrado contido na lista de ids.");
                return new List<EMUser>();
            }

            return await _userManager.Users
                .Where(u => ids.Contains(u.Id))
                .ToListAsync();
        }

        //V2 Implemented
        public async Task<List<EMUser>> GetUsersByRoleAsync(string role)
        {
            _logger.LogInformation("BD: a obter utilizadores com o papel {Role}.", role);

            var users = await _userManager.GetUsersInRoleAsync(role);

            if (users == null || !users.Any())
            {
                _logger.LogWarning("Nenhum utilizador encontrado com o papel {Role}.", role);
                return new List<EMUser>();
            }

            return users.ToList();
        }

        //V2 Implemented
        public async Task SetUserActiveStatusAsync(string userId, bool isActive)
        {
            _logger.LogInformation(
                "BD: a alterar estado do utilizador {UserId} para {IsActive}.",
                userId,
                isActive);

            var user = await _userManager.FindByIdAsync(userId);

            if (user is null)
            {
                _logger.LogWarning(
                    "Utilizador {UserId} não encontrado ao tentar alterar estado.",
                    userId);

                return;
            }

            user.IsActive = isActive;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                _logger.LogError(
                    "Falha ao atualizar estado do utilizador {UserId}. Erros: {Errors}",
                    userId,
                    string.Join(", ", result.Errors.Select(e => e.Description)));

                return;
            }

            _logger.LogInformation(
                "Estado do utilizador {UserId} atualizado com sucesso para {IsActive}.",
                userId,
                isActive);
        }

        //V2 Implemented
        public async Task UpdateUserAsync(EMUser user)
        {
            _logger.LogInformation("BD: atualização de utilizador {UserId}.", user.Id);

            var existing = await _userManager.FindByIdAsync(user.Id);

            if (existing is null)
            {
                _logger.LogWarning("User {UserId} não encontrado.", user.Id);
                return;
            }

            existing.FirstName = user.FirstName;
            existing.LastName = user.LastName;
            existing.PhoneNumber = user.PhoneNumber;
            existing.Address = user.Address;
            existing.Birthdate = user.Birthdate;
            existing.IsActive = user.IsActive;

            await _userManager.UpdateAsync(existing);

        }

        //V2 Implemented
        public async Task<EMUser?> GetByIdAsync(string id)
        {
            return await _userManager.Users
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        //V2 Implemented
        public async Task CreateUserAsync(EMUser user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }





    }
}