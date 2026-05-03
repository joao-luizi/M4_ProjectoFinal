using RepositoryLibrary.Models;

namespace RepositoryLibrary.IRepository
{
    public interface IPackageRepository
    {
        public Task<Package> GetPackageByIdAsync(int id);
        public Task<List<Package>> GetPackageByUserIdAsync(string userId);
        public Task<Package> AddNewPackageAsync(Package package);
        public Task<Package> EditPackageAsync(Package package);
        public Task<Package> DeletePackageByIdAsync(int packageId);

    }
}
