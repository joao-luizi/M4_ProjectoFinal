using System.Data;
using RepositoryLibrary.IRepository;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using SharedLibrary;
using RepositoryLibrary.Models.DTOs;

namespace RepositoryLibrary.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly EM_DbContext _emContext;
        private readonly UserManager<EMUser> _userManager;

        public UserRepository(EM_DbContext emContext, UserManager<EMUser> userManager)
        {
            _emContext = emContext;
            _userManager = userManager;
        }

        public async Task<List<UpdateUserDto>> GetAllUsers(int schoolId)
        {
            try
            {
                List<SchoolUser> schoolUsers = await _emContext.SchoolUsers.Where(scl => scl.SchoolId == schoolId).ToListAsync();
                List<EMUser> eMUsers = new List<EMUser>();
                List<UpdateUserDto> listOfUsers = new List<UpdateUserDto>();

                foreach(SchoolUser schoolUser in schoolUsers)
                {
                    var eMUser = await _userManager.FindByIdAsync(schoolUser.UserId);

                    if (eMUser is not null)
                    {
                        eMUsers.Add(eMUser);
                    }
                }

                foreach(EMUser user in eMUsers)
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

                return listOfUsers;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UserDTO> GetUserById(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);


                if (user is null)
                {
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

                return newUser;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<List<UpdateUserDto>> GetUsersByRole(string role)
        {
            try
            {
                List<UpdateUserDto> listToReturn = new List<UpdateUserDto>();
                var usersList = await _userManager.GetUsersInRoleAsync(role);

                if(usersList.IsNullOrEmpty())
                {
                    throw new Exception($"There are no users with the role {role}");
                }

                foreach(EMUser user in usersList)
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

                return listToReturn;
            }
            catch(Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UserDTO> DeleteUserAsync(string id)
        {
            try
            {
                var userToDelete = await _userManager.FindByIdAsync(id);

                if (userToDelete is null)
                {
                    throw new Exception($"The user does not exist. Id = {id}");
                }

                UserDTO userDeleted = new UserDTO
                {
                    UserName = userToDelete.UserName,
                    Email = userToDelete.Email,
                    PhoneNumber = userToDelete.PhoneNumber,
                    Address = userToDelete.Address,
                    Birthdate = userToDelete.Birthdate,
                    CitizenNumber = userToDelete.CitizenNumber,
                    SocialHealthNumber = userToDelete.SocialHealthNumber,
                    TaxIdentificationNumber = userToDelete.TaxIdentificationNumber
                };

                var result = await _userManager.DeleteAsync(userToDelete);

                if (!result.Succeeded)
                {
                    throw new Exception($"There was an error while trying to delete the user with Id = {id}");
                }

                return userDeleted;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UpdateUserDto> GetEditUserAsync(string id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id);

                if (user is null)
                {
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

                return userToUpdate;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<UpdateUserDto> EditUserAsync(UpdateUserDto user)
        {
            try
            {
                var userToUpdate = await _userManager.FindByIdAsync(user.Id);


                if (userToUpdate is null)
                {
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
                    throw new Exception($"There was an error while updating {userToUpdate.FirstName} {userToUpdate.LastName}");
                }

                return updatedUser;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        private async Task<IList<string>> ChangeUserRole(UpdateUserDto userToUpdate, string? oldRole, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userToUpdate.Id);

            if (!oldRole.IsNullOrEmpty())
            {
                var roleResult = await _userManager.RemoveFromRoleAsync(user, oldRole);

                if (!roleResult.Succeeded)
                {
                    throw new Exception($"There was an error while changing user with Id = {user.Id} role.");
                }
            }

            var newRoleResult = await _userManager.AddToRoleAsync(user, newRole);

            if (!newRoleResult.Succeeded)
            {
                throw new Exception($"There was an error while adding a new role to user with Id = {user.Id}.");
            }

            var updatedRole = await _userManager.GetRolesAsync(user);

            return updatedRole;
        }

        public async Task<IList<string>> GetUserRole(string userId)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user is null)
                {
                    throw new Exception($"The user with Id = {userId} was not found.");
                }

                var role = await _userManager.GetRolesAsync(user);

                return role;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<Photo> AddUserPhotoAsync(string userId, string filepath)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user is null)
                {
                    throw new Exception($"The user with Id = {userId} does not exist.");
                }

                byte[] image = await File.ReadAllBytesAsync(filepath);

                if(image.IsNullOrEmpty())
                {
                    throw new Exception($"There was an error trying to convert the image with the filepath: {filepath}");
                }

                Photo photo = new Photo
                {
                    UserId = user.Id,
                    UserPhoto = image
                };

                await _emContext.Photos.AddAsync(photo);
                _emContext.SaveChanges();

                return photo;
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
                var userPhoto = await _emContext.Photos.FirstOrDefaultAsync(usr => usr.UserId == userId);

                if (userPhoto is null)
                {
                    throw new Exception($"The user with Id = {userId} does not have a photo yet.");
                }

                byte[] image = await File.ReadAllBytesAsync(filepath);

                if (image.IsNullOrEmpty())
                {
                    throw new Exception($"There was an error trying to convert the image with the filepath: {filepath}");
                }

                userPhoto.UserPhoto = image;

                await _emContext.Photos.AddAsync(userPhoto);
                _emContext.SaveChanges();

                return userPhoto;
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
                var userPhoto = await _emContext.Photos.FirstOrDefaultAsync(usr => usr.UserId == userId);

                if (userPhoto is null)
                {
                    throw new Exception($"The user with Id = {userId} does not have a photo yet.");
                }

                _emContext.Photos.Remove(userPhoto);
                _emContext.SaveChanges();

                return userPhoto;
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
                var userPhoto = await _emContext.Photos.FirstOrDefaultAsync(usr => usr.UserId == userId);

                if (userPhoto is null)
                {
                    throw new Exception($"The user with Id = {userId} does not have a photo yet.");
                }

                return userPhoto;
            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }
    }
}
