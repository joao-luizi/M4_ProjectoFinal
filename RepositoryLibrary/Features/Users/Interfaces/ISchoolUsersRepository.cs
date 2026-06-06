using RepositoryLibrary.Features.Purchases.DTOs;
using RepositoryLibrary.Features.Users.DTOs;
using RepositoryLibrary.Features.Users.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Users.Interfaces
{
    public interface ISchoolUsersRepository
    {

        //V2 implemented
        Task<List<SchoolUser>> GetAllWithIncludesAsync();
        Task<bool> ExistsAsync(string userId, int schoolId);

        Task AddAsync(SchoolUser entity);

        Task<SchoolUser?> GetAsync(string userId, int schoolId);

        Task DeleteAsync(SchoolUser entity);

        Task<List<UserListDto>> GetUsersBySchoolAsync(int schoolId);

        Task<List<UserSchoolDto>> GetSchoolsByUserAsync(string userId);
    }
}
