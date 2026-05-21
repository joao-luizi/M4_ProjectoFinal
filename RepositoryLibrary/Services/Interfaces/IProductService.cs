

using RepositoryLibrary.Models;

namespace RepositoryLibrary.Services
{
    public interface IProductService
    {
        Task<List<Product>> GetAll();
        Task<Product?> GetById(int id);
        Task Create(Product product);

       

    }
}
