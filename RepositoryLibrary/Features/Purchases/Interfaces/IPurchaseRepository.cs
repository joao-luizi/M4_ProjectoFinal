using RepositoryLibrary.Features.Purchases.DTOs;
using RepositoryLibrary.Features.Purchases.Entities;

namespace RepositoryLibrary.Features.Purchases.Interfaces
{
    public interface IPurchaseRepository
    {
        //V2 Implemented
        Task<List<PurchaseCountByUser>> GetPurchaseCountsByUserAsync();
        //V2 Implemented
        Task AddAsync(Purchase purchase);
        //V2 Implemented
        Task UpdateAsync(Purchase purchase);
        //V2 Implemented
        Task<Purchase?> GetByIdAsync(int id);
        //V2 Implemented
        Task<Purchase?> GetByIdWithLinesAsync(int id);
        //V2 Implemented
        Task<List<Purchase>> GetByUserIdAsync(string userId);
        //V2 Implemented
        Task<List<Purchase>> GetByUserIdWithLinesAsync(string userId);
       


    }
}
