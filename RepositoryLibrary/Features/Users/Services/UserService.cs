using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Bookings.Interfaces;
using RepositoryLibrary.Features.Bookings.Repositories;
using RepositoryLibrary.Features.Horses.DTOs;
using RepositoryLibrary.Features.Horses.Interfaces;
using RepositoryLibrary.Features.Horses.Repositories;
using RepositoryLibrary.Features.Images.Interfaces;
using RepositoryLibrary.Features.Lessons.Interfaces;
using RepositoryLibrary.Features.Lessons.Repositories;
using RepositoryLibrary.Features.Schools.Interfaces;
using RepositoryLibrary.Features.Schools.Repositories;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using RepositoryLibrary.Features.Users.Repositories;
using RepositoryLibrary.Features.Users.Repository;


namespace RepositoryLibrary.Features.Users.Service
{

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        private readonly IUserPhotoRepository _userPhotoRepo;
        private readonly IImageService _imageService;
        private readonly ISchoolUsersRepository _schoolUserRepo;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepo,
            IUserPhotoRepository userPhotoRepo,
            ISchoolUsersRepository schoolUserRepo,
            IImageService imageService,
            ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _userPhotoRepo = userPhotoRepo;
            _schoolUserRepo = schoolUserRepo;
            _imageService = imageService;
            _logger = logger;
        }

        //V2 Implemented
        public async Task<AdminUsersDto> GetAdminUsersAsync()
        {
            var users = (await _userRepo.GetUsersForAdministrationAsync()).ToList();

            return new AdminUsersDto
            {
                Users = users
                    .Select(x => new AdminUserListItemDto
                    {
                        Id = x.Id,
                        FullName = $"{x.FirstName} {x.LastName}",
                        Email = x.Email ?? string.Empty,
                        PhoneNumber = x.PhoneNumber,
                        Role = x.Role,
                        IsActive = x.IsActive,
                        PhotoPath = x.PhotoPath
                    })
                    .ToList(),

                TotalUsers = users.Count,
                ActiveUsers = users.Count(x => x.IsActive),
                InactiveUsers = users.Count(x => !x.IsActive)
            };
        }

        //V2 Implemented
        public async Task ActivateUserAsync(string userId)
        {
            await _userRepo.SetUserActiveStatusAsync(
                userId,
                true);
        }
        //V2 Implemented
        public async Task DeactivateUserAsync(string userId)
        {
            await _userRepo.SetUserActiveStatusAsync(
                userId,
                false);
        }
        //V2 Implemented GetUsersBySchoolAndRole
        public async Task<List<EMUser>> GetUsersBySchoolAndRole(int schoolId, string role)
        {
            try
            {
                var users = await _userRepo.GetUsersByRoleAsync(role);
                var schoolUsers = await _schoolUserRepo.GetUsersBySchoolAsync(schoolId);

                var schoolUserIds = schoolUsers
                .Select(su => su.UserId)
                .ToHashSet();

                return users.Where(u => schoolUserIds.Contains(u.Id)).ToList();

            }
            catch (Exception e)
            {
                throw new Exception(e.Message, e.InnerException);
            }
        }

        public async Task<AdminUserDetailsDto?> GetUserDetailsAsync(string userId)
        {
            var users = await _userRepo.GetUsersForAdministrationAsync();

            var user = users.FirstOrDefault(x => x.Id == userId);

            if (user is null)
                return null;

            return new AdminUserDetailsDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                Role = user.Role,
                IsActive = user.IsActive,
                PhotoPath = user.PhotoPath
            };
        }

        private async Task UpdateUserAsync(AdminUserDetailsDto user)
        {
            if (user is null)
                return;

            var existingUser = await _userRepo.GetByIdAsync(user.Id);

            if (existingUser is null)
                return;

            // 1. Atualizar dados base
            existingUser.FirstName = user.FirstName;
            existingUser.LastName = user.LastName;
            existingUser.PhoneNumber = user.PhoneNumber;
            existingUser.Address = user.Address;
            existingUser.Birthdate = user.Birthdate;
            existingUser.IsActive = user.IsActive;

            await _userRepo.UpdateUserAsync(existingUser);

            // 2. Se não há imagem nova, termina aqui
            if (user.NewPhoto is null)
                return;

            // 3. Obter foto atual (se existir)
            var existingPhoto = await _userPhotoRepo.GetByUserIdAsync(user.Id);

            string newPath = await _imageService.ReplaceImageAsync(
                user.NewPhoto,
                folder: "users",
                fileName: user.Id,
                existingImagePath: existingPhoto?.FotoPath);

            // 4. Persistir relação UserPhoto
            if (existingPhoto is null)
            {
                await _userPhotoRepo.AddAsync(new UserPhoto
                {
                    UserId = user.Id,
                    FotoPath = newPath
                });
            }
            else
            {
                existingPhoto.FotoPath = newPath;
                await _userPhotoRepo.SaveChangesAsync();
            }
        }

        private async Task CreateUserAsync(AdminUserDetailsDto dto)
        {
            //primeiro gravar o user porque o user id é usado na foto
            //Fazer na feature Horses CreateHorseAsync
        }
        //V2 Implelmented
        public async Task SaveUserAsync(AdminUserDetailsDto dto)
        {
            if (dto.Id == "")
            {
                // create
                await CreateUserAsync(dto);
            }
            else
            {
                // update
                await UpdateUserAsync(dto);
            }
        }



    }
}