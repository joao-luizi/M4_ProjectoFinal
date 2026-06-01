using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;

namespace RepositoryLibrary.Features.Users.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly RideReadyDbContext _emContext;
        private readonly UserManager<EMUser> _userManager;
        private readonly ILogger<UserRepository> _logger;

        public UserRepository(RideReadyDbContext emContext, UserManager<EMUser> userManager, ILogger<UserRepository> logger)
        {
            _emContext = emContext;
            _userManager = userManager;
            _logger = logger;
        }

        public async Task<List<UpdateUserDto>> GetAllUsers(int schoolId)
        {
            _logger.LogInformation("BD: a obter todos os utilizadores da escola {SchoolId}.", schoolId);
            try
            {
                List<SchoolUser> schoolUsers = await _emContext.SchoolUsers.Where(scl => scl.SchoolId == schoolId).ToListAsync();
                List<EMUser> eMUsers = new List<EMUser>();
                List<UpdateUserDto> listOfUsers = new List<UpdateUserDto>();

                foreach (SchoolUser schoolUser in schoolUsers)
                {
                    var eMUser = await _userManager.FindByIdAsync(schoolUser.UserId);

                    if (eMUser is not null)
                    {
                        eMUsers.Add(eMUser);
                    }
                }

                foreach (EMUser user in eMUsers)
                {
                    var role = await _userManager.GetRolesAsync(user);
                    Photo? photo = await _emContext.Photos.FirstOrDefaultAsync(pht => pht.UserId == user.Id);

                    UpdateUserDto userToAdd = new UpdateUserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Address = user.Address,
                        PhoneNumber = user.PhoneNumber,
                        Birthdate = user.Birthdate,
                        IsActive = user.IsActive,
                        TaxIdentificationNumber = user.TaxIdentificationNumber,
                        SocialHealthNumber = user.SocialHealthNumber,
                        CitizenNumber = user.CitizenNumber,
                        Roles = role,
                        Photo = photo
                    };

                    listOfUsers.Add(userToAdd);
                }

                _logger.LogInformation("BD: obtidos {Count} utilizadores para a escola {SchoolId}.", listOfUsers.Count, schoolId);
                return listOfUsers;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter utilizadores da escola {SchoolId}.", schoolId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UserDTO> GetUserById(string id)
        {
            _logger.LogInformation("BD: a consultar utilizador {UserId}.", id);
            try
            {
                var user = await _userManager.FindByIdAsync(id);


                if (user is null)
                {
                    _logger.LogWarning("Utilizador {UserId} não encontrado.", id);
                    throw new Exception("The specified user does not exist");
                }

                var photo = await _emContext.Photos.FirstOrDefaultAsync(pht => pht.UserId == user.Id);

                UserDTO newUser = new UserDTO
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber,
                    Address = user.Address,
                    Birthdate = user.Birthdate,
                    CitizenNumber = user.CitizenNumber,
                    SocialHealthNumber = user.SocialHealthNumber,
                    TaxIdentificationNumber = user.TaxIdentificationNumber
                };

                if (photo is not null)
                {
                    newUser.Photo = photo;
                }

                _logger.LogInformation("BD: utilizador {UserId} obtido com sucesso.", id);
                return newUser;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao consultar utilizador {UserId}.", id);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<UpdateUserDto>> GetUsersByRole(string role)
        {
            _logger.LogInformation("BD: a obter utilizadores com o papel {Role}.", role);
            try
            {
                List<UpdateUserDto> listToReturn = new List<UpdateUserDto>();
                var usersList = await _userManager.GetUsersInRoleAsync(role);

                if (usersList.IsNullOrEmpty())
                {
                    _logger.LogWarning("Nenhum utilizador encontrado com o papel {Role}.", role);
                    throw new Exception($"There are no users with the role {role}");
                }

                foreach (EMUser user in usersList)
                {
                    var userRole = await _userManager.GetRolesAsync(user);
                    var userPhoto = await _emContext.Photos.FirstOrDefaultAsync(pht => pht.UserId == user.Id);

                    UpdateUserDto newUser = new UpdateUserDto
                    {
                        Id = user.Id,
                        Email = user.Email,
                        Name = user.UserName,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Address = user.Address,
                        PhoneNumber = user.PhoneNumber,
                        Birthdate = user.Birthdate,
                        IsActive = user.IsActive,
                        TaxIdentificationNumber = user.TaxIdentificationNumber,
                        SocialHealthNumber = user.SocialHealthNumber,
                        CitizenNumber = user.CitizenNumber,
                        Roles = userRole,
                        Photo = userPhoto
                    };

                    listToReturn.Add(newUser);
                }

                _logger.LogInformation("BD: obtidos {Count} utilizadores com o papel {Role}.", listToReturn.Count, role);
                return listToReturn;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter utilizadores com o papel {Role}.", role);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task InactivateUser(string id)
        {
            _logger.LogInformation("BD: a inativar utilizador {UserId}.", id);

            var userToInactivate = await _userManager.FindByIdAsync(id);

            if (userToInactivate is null)
            {
                _logger.LogWarning("Não foi possível inativar: utilizador {UserId} não existe.", id);
                throw new Exception($"The user does not exist. Id = {id}");
            }

            userToInactivate.IsActive = false;

            var result = await _userManager.UpdateAsync(userToInactivate);

            if (!result.Succeeded)
            {
                _logger.LogWarning("Falha ao inativar utilizador {UserId}: {Errors}.", id, string.Join(", ", result.Errors.Select(e => e.Description)));
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            _logger.LogInformation("BD: utilizador {UserId} inativado com sucesso.", id);
        }

        public async Task<UpdateUserDto> GetEditUserAsync(string id)
        {
            _logger.LogInformation("BD: a obter utilizador {UserId} para edição.", id);
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user is null)
                {
                    _logger.LogWarning("Utilizador {UserId} não encontrado para edição.", id);
                    throw new Exception($"The user with Id = {id}");
                }

                // this is to get all the roles to show in the view;
                // to change the role of a user, i need to remove the one that he has first and than add a new role.
                var roles = await _userManager.GetRolesAsync(user);

                var photo = await _emContext.Photos.FirstOrDefaultAsync(pht => pht.UserId == user.Id);

                var userToUpdate = new UpdateUserDto
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.UserName,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Address = user.Address,
                    Birthdate = user.Birthdate,
                    PhoneNumber = user.PhoneNumber,
                    IsActive = user.IsActive,
                    TaxIdentificationNumber = user.TaxIdentificationNumber,
                    SocialHealthNumber = user.SocialHealthNumber,
                    CitizenNumber = user.CitizenNumber,
                    Roles = roles,
                    Photo = photo
                };

                _logger.LogInformation("BD: utilizador {UserId} obtido para edição.", id);
                return userToUpdate;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter utilizador {UserId} para edição.", id);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UpdateUserDto> EditUserAsync(UpdateUserDto user)
        {
            _logger.LogInformation("BD: a editar utilizador {UserId}.", user.Id);
            try
            {
                var userToUpdate = await _userManager.FindByIdAsync(user.Id);


                if (userToUpdate is null)
                {
                    _logger.LogWarning("Utilizador {UserId} não encontrado para edição.", user.Id);
                    throw new Exception($"The user with Id = {user.Id} was not found.");
                }

                var roles = await _userManager.GetRolesAsync(userToUpdate);

                var newRole = await ChangeUserRole(user, roles.FirstOrDefault(), user.Roles.First());

                var photoToUpdate = await _emContext.Photos.FirstOrDefaultAsync(pht => pht.UserId == user.Id);

                userToUpdate.Email = user.Email;
                userToUpdate.UserName = user.Name;
                userToUpdate.FirstName = user.FirstName;
                userToUpdate.LastName = user.LastName;
                userToUpdate.Address = user.Address;
                userToUpdate.Birthdate = user.Birthdate;
                userToUpdate.PhoneNumber = user.PhoneNumber;
                userToUpdate.IsActive = user.IsActive;
                userToUpdate.TaxIdentificationNumber = user.TaxIdentificationNumber;
                userToUpdate.SocialHealthNumber = user.SocialHealthNumber;
                userToUpdate.CitizenNumber = user.CitizenNumber;
                photoToUpdate = user.Photo;


                var updatedUser = new UpdateUserDto
                {
                    Id = userToUpdate.Id,
                    Email = userToUpdate.Email,
                    Name = userToUpdate.UserName,
                    FirstName = userToUpdate.FirstName,
                    LastName = userToUpdate.LastName,
                    Address = userToUpdate.Address,
                    Birthdate = userToUpdate.Birthdate,
                    PhoneNumber = userToUpdate.PhoneNumber,
                    IsActive = userToUpdate.IsActive,
                    TaxIdentificationNumber = userToUpdate.TaxIdentificationNumber,
                    SocialHealthNumber = userToUpdate.SocialHealthNumber,
                    CitizenNumber = userToUpdate.CitizenNumber,
                    Roles = newRole,
                    Photo = photoToUpdate
                };

                var result = await _userManager.UpdateAsync(userToUpdate);

                if (!result.Succeeded)
                {
                    _logger.LogWarning("Falha ao gravar edição do utilizador {UserId}.", user.Id);
                    throw new Exception($"There was an error while updating {userToUpdate.FirstName} {userToUpdate.LastName}");
                }

                _logger.LogInformation("BD: utilizador {UserId} editado com sucesso.", user.Id);
                return updatedUser;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao editar utilizador {UserId}.", user.Id);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        private async Task<IList<string>> ChangeUserRole(UpdateUserDto userToUpdate, string? oldRole, string newRole)
        {
            _logger.LogInformation("BD: a alterar papel do utilizador {UserId} de '{OldRole}' para '{NewRole}'.", userToUpdate.Id, oldRole ?? "(nenhum)", newRole);

            var user = await _userManager.FindByIdAsync(userToUpdate.Id);

            if (!oldRole.IsNullOrEmpty())
            {
                var roleResult = await _userManager.RemoveFromRoleAsync(user, oldRole);

                if (!roleResult.Succeeded)
                {
                    _logger.LogWarning("Falha ao remover papel '{OldRole}' do utilizador {UserId}.", oldRole, user.Id);
                    throw new Exception($"There was an error while changing user with Id = {user.Id} role.");
                }
            }

            var newRoleResult = await _userManager.AddToRoleAsync(user, newRole);

            if (!newRoleResult.Succeeded)
            {
                _logger.LogWarning("Falha ao adicionar papel '{NewRole}' ao utilizador {UserId}.", newRole, user.Id);
                throw new Exception($"There was an error while adding a new role to user with Id = {user.Id}.");
            }

            var updatedRole = await _userManager.GetRolesAsync(user);

            _logger.LogInformation("BD: papel do utilizador {UserId} alterado para '{NewRole}'.", user.Id, newRole);
            return updatedRole;
        }

        public async Task<IList<string>> GetUserRole(string userId)
        {
            _logger.LogInformation("BD: a obter papéis do utilizador {UserId}.", userId);
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user is null)
                {
                    _logger.LogWarning("Utilizador {UserId} não encontrado ao consultar papéis.", userId);
                    throw new Exception($"The user with Id = {userId} was not found.");
                }

                var role = await _userManager.GetRolesAsync(user);

                _logger.LogInformation("BD: obtidos {Count} papéis para o utilizador {UserId}.", role.Count, userId);
                return role;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter papéis do utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Photo> AddUserPhotoAsync(string userId, string filepath)
        {
            _logger.LogInformation("BD: a adicionar foto ao utilizador {UserId} a partir de '{FilePath}'.", userId, filepath);
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user is null)
                {
                    _logger.LogWarning("Não foi possível adicionar foto: utilizador {UserId} não existe.", userId);
                    throw new Exception($"The user with Id = {userId} does not exist.");
                }

                byte[] image = await File.ReadAllBytesAsync(filepath);

                if (image.IsNullOrEmpty())
                {
                    _logger.LogWarning("Ficheiro '{FilePath}' vazio ao adicionar foto ao utilizador {UserId}.", filepath, userId);
                    throw new Exception($"There was an error trying to convert the image with the filepath: {filepath}");
                }

                Photo photo = new Photo
                {
                    UserId = user.Id,
                    UserPhoto = image
                };

                await _emContext.Photos.AddAsync(photo);
                _emContext.SaveChanges();

                _logger.LogInformation("BD: foto adicionada ao utilizador {UserId} ({Size} bytes).", userId, image.Length);
                return photo;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao adicionar foto ao utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Photo> UpdateUserPhotoAsync(string userId, string filepath)
        {
            _logger.LogInformation("BD: a atualizar foto do utilizador {UserId} a partir de '{FilePath}'.", userId, filepath);
            try
            {
                var userPhoto = await _emContext.Photos.FirstOrDefaultAsync(usr => usr.UserId == userId);

                if (userPhoto is null)
                {
                    _logger.LogWarning("Utilizador {UserId} ainda não tem foto; nada a atualizar.", userId);
                    throw new Exception($"The user with Id = {userId} does not have a photo yet.");
                }

                byte[] image = await File.ReadAllBytesAsync(filepath);

                if (image.IsNullOrEmpty())
                {
                    _logger.LogWarning("Ficheiro '{FilePath}' vazio ao atualizar foto do utilizador {UserId}.", filepath, userId);
                    throw new Exception($"There was an error trying to convert the image with the filepath: {filepath}");
                }

                userPhoto.UserPhoto = image;

                await _emContext.Photos.AddAsync(userPhoto);
                _emContext.SaveChanges();

                _logger.LogInformation("BD: foto do utilizador {UserId} atualizada ({Size} bytes).", userId, image.Length);
                return userPhoto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao atualizar foto do utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Photo> DeleteUserPhotoAsync(string userId)
        {
            _logger.LogInformation("BD: a eliminar foto do utilizador {UserId}.", userId);
            try
            {
                var userPhoto = await _emContext.Photos.FirstOrDefaultAsync(usr => usr.UserId == userId);

                if (userPhoto is null)
                {
                    _logger.LogWarning("Utilizador {UserId} ainda não tem foto; nada a eliminar.", userId);
                    throw new Exception($"The user with Id = {userId} does not have a photo yet.");
                }

                _emContext.Photos.Remove(userPhoto);
                _emContext.SaveChanges();

                _logger.LogInformation("BD: foto do utilizador {UserId} eliminada.", userId);
                return userPhoto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao eliminar foto do utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Photo> GetUserPhotoAsync(string userId)
        {
            _logger.LogInformation("BD: a obter foto do utilizador {UserId}.", userId);
            try
            {
                var userPhoto = await _emContext.Photos.FirstOrDefaultAsync(usr => usr.UserId == userId);

                if (userPhoto is null)
                {
                    _logger.LogWarning("Utilizador {UserId} não tem foto.", userId);
                    throw new Exception($"The user with Id = {userId} does not have a photo yet.");
                }

                _logger.LogInformation("BD: foto do utilizador {UserId} obtida.", userId);
                return userPhoto;
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Erro ao obter foto do utilizador {UserId}.", userId);
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}