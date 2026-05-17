using RepositoryLibrary.IRepository;
using RepositoryLibrary.IServices;
using RepositoryLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Services
{
    public class PackageService : IPackageService
    {
        private readonly IPackageRepository _packageRepository;
        public PackageService(IPackageRepository packageRepository) 
        { 
            _packageRepository = packageRepository;
        }

        public async Task<List<Package>> GetAllPackages()
        {
            return await _packageRepository.GetAllAsync();
        }
    }
}
