

using Microsoft.Extensions.Logging;
using RepositoryLibrary.Features.Images.Interfaces;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using Microsoft.AspNetCore.Identity;



namespace RepositoryLibrary.Features.Users.Service
{

    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepo;
        
        private readonly IUserFotoRepository _userPhotoRepo;
        private readonly IImageService _imageService;
        private readonly ISchoolUsersRepository _schoolUserRepo;
        private readonly UserManager<EMUser> _userManager;
        private readonly ILogger<UserService> _logger;

        public UserService(
            IUserRepository userRepo,
            IUserFotoRepository userPhotoRepo,
            ISchoolUsersRepository schoolUserRepo,
            IImageService imageService,
            UserManager<EMUser> userManager,
            ILogger<UserService> logger)
        {
            _userRepo = userRepo;
            _userPhotoRepo = userPhotoRepo;
            _schoolUserRepo = schoolUserRepo;
            _imageService = imageService;
            _userManager = userManager;
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
        public async Task<List<SelectTeacherDto>> GetSelectUsersBySchoolAndRole(int schoolId, string role)
        {
            try
            {
                var users = await _userRepo.GetUsersByRoleAsync(role);
                var schoolUsers = await _schoolUserRepo.GetUsersBySchoolAsync(schoolId);

                var schoolUserIds = schoolUsers
                .Select(su => su.UserId)
                .ToHashSet();

                return users
                  .Where(u => schoolUserIds.Contains(u.Id))
                  .Select(u => new SelectTeacherDto
                  {
                      Id = u.Id,
                      Name = u.FirstName + " " + u.LastName + "(" + u.UserName + ")"
                  })
                  .ToList();

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
                await _userPhotoRepo.AddAsync(new UserFoto
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
            var user = new EMUser
            {
                UserName = dto.Email,
                Email = dto.Email,

                FirstName = dto.FirstName,
                LastName = dto.LastName,
                PhoneNumber = dto.PhoneNumber,

                Birthdate = dto.Birthdate,
                Address = dto.Address,

                IsActive = dto.IsActive,
                RegisterDate = DateTime.UtcNow
            };

            // 1. Identity (AspNetUsers)
            var result = await _userManager.CreateAsync(user, dto.Password!);

            if (!result.Succeeded)
                throw new Exception(string.Join(", ", result.Errors.Select(e => e.Description)));

            // 2. Role (Identity)
            if (!string.IsNullOrWhiteSpace(dto.Role))
            {
                await _userManager.AddToRoleAsync(user, dto.Role);
            }

           

            // 4. Photo (RideReadyDB)
            if (dto.NewPhoto is not null)
            {
                var photoPath = await _imageService.SaveImageAsync(dto.NewPhoto, "users", user.Id);

                var photo = new UserFoto
                {
                    UserId = user.Id,
                    FotoPath = photoPath
                };

                await _userPhotoRepo.AddAsync(photo);
            }
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

        //V2 Implemented 
        public async Task<List<AdminUserListItemDto>> GetUserListItemsAsync()
        {
            var users = (await _userRepo.GetUsersForAdministrationAsync()).ToList();


            return users
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
                 .ToList();

                
           
        }



    }
}