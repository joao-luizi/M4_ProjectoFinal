using RepositoryLibrary.Features.Products.Entities;

namespace RepositoryLibrary.Features.Products.Interfaces
{
    public interface IProductService
    {
        Task<List<Product>> GetAll();
        Task<Product?> GetById(int id);
        Task Create(Product product);

       

    }
}
