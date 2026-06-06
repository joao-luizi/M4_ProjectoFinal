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
        
        //V2 Implemented
        Task<List<SchoolUser>> GetUsersBySchoolAsync(int schoolId);
        Task<bool> ExistsAsync(string userId, int schoolId);

        Task AddAsync(SchoolUser entity);

        Task<SchoolUser?> GetAsync(string userId, int schoolId);

        Task DeleteAsync(SchoolUser entity);

        Task<List<UserSchoolDto>> GetSchoolsByUserAsync(string userId);
    }
}
