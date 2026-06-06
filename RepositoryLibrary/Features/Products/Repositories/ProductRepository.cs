using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Products.Entities;
using RepositoryLibrary.Features.Products.Interfaces;



namespace RepositoryLibrary.Features.Products.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly RideReadyDbContext _context;
        private ILogger<ProductRepository> _logger;
        public ProductRepository(RideReadyDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        //V2 Implemented
        public async Task<List<Product>> GetByIdsAsync(List<int> productIds)
        {
            _logger.LogInformation("A obter produtos por ids. Count={Count}", productIds.Count);

            try
            {
                var products = await _context.Products
                    .Where(p => productIds.Contains(p.Id))
                    .ToListAsync();

                _logger.LogInformation("Obtidos {Count} produtos.", products.Count);

                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter produtos por ids.");
                throw;
            }
        }
    }
}
