using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Purchases.DTOs;
using RepositoryLibrary.Features.Purchases.Entities;
using RepositoryLibrary.Features.Purchases.Interfaces;
using RideReady.Data;

namespace RepositoryLibrary.Features.Purchases.Repositories
{
    public  class PurchaseRepository : IPurchaseRepository
    {
        private readonly RideReadyDbContext _context;
        private readonly AppIdentityDbContext _identity;
        private readonly ILogger<PurchaseRepository> _logger;
        public PurchaseRepository(RideReadyDbContext context, AppIdentityDbContext identity, ILogger<PurchaseRepository> logger)
        {
            _context = context;
            _identity = identity;
            _logger = logger;
        }

        //V2 Implemented
        public async Task<List<PurchaseCountByUser>> GetPurchaseCountsByUserAsync()
        {
            return await _context.Purchases
                .GroupBy(p => p.UserId)
                .Select(g => new PurchaseCountByUser
                {
                    UserId = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
        }

        //V2 Implemented
        public async Task AddAsync(Purchase purchase)
        {
            await _context.Purchases.AddAsync(purchase);
            await _context.SaveChangesAsync();
        }

        //V2 Implemented
        public async Task UpdateAsync(Purchase purchase)
        {
            _context.Purchases.Update(purchase);
            await _context.SaveChangesAsync();
        }

        //V2 Implemented
        public async Task<Purchase?> GetByIdAsync(int id)
        {
            return await _context.Purchases
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        //V2 Implemented
        public async Task<Purchase?> GetByIdWithLinesAsync(int id)
        {
            return await _context.Purchases
                .Include(p => p.Lines)
                    .ThenInclude(l => l.Product)
                        .ThenInclude(p => p.Entitlements)
                            .ThenInclude(e => e.LessonType)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        //V2 Implemented
        public async Task<List<Purchase>> GetByUserIdAsync(string userId)
        {
            return await _context.Purchases
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.PurchasedAtUtc)
                .ToListAsync();
        }

        //V2 Implemented
        public async Task<List<Purchase>> GetByUserIdWithLinesAsync(string userId)
        {
            return await _context.Purchases
                .Where(x => x.UserId == userId)
                .Include(p => p.Lines)
                    .ThenInclude(l => l.Product)
                .OrderByDescending(x => x.PurchasedAtUtc)
                .ToListAsync();
        }


    }
}
