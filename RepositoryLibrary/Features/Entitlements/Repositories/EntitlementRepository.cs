using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Entitlements.Entities;
using RepositoryLibrary.Features.Entitlements.Enums;
using RepositoryLibrary.Features.Entitlements.Interfaces;


namespace RepositoryLibrary.Features.Entitlements.Repositories
{
    public class EntitlementRepository : IEntitlementRepository
    {
        private readonly RideReadyDbContext _emContext;
        private readonly ILogger<EntitlementRepository> _logger;

        public EntitlementRepository(
            RideReadyDbContext context,
            ILogger<EntitlementRepository> logger)
        {
            _emContext = context;
            _logger = logger;
        }

       

        //V2 Implemented
        public async Task<List<UserCreditLedgerEntry>> GetCreditLedgerAsync(string userId)
        {
            try
            {
                return await _emContext.UserCreditLedgerEntries
                    .AsNoTracking()
                    .Include(l => l.Product)
                    .Include(l => l.LessonType)
                    .Where(l => l.UserId == userId)
                    .OrderByDescending(l => l.CreatedAtUtc)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching credit ledger for user {UserId}", userId);
                throw;
            }
        }


        //V2 Implemented
        public async Task<List<UserSubscriptionEntitlement>> GetSubscriptionEntitlementsAsync(string userId)
        {
            try
            {
                return await _emContext.UserSubscriptionEntitlements
                    .AsNoTracking()
                    .Include(e => e.UserSubscription)
                    .Include(e => e.LessonType)
                    .Where(e =>
                        e.UserSubscription.UserId == userId &&
                        e.UserSubscription.Status == SubscriptionStatus.Active &&
                        e.UserSubscription.PeriodStart <= DateOnly.FromDateTime(DateTime.UtcNow) &&
                        e.UserSubscription.PeriodEnd >= DateOnly.FromDateTime(DateTime.UtcNow))
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching subscription entitlements for user {UserId}", userId);
                throw;
            }
        }

        //V2 Implemented
        public async Task AddAsync(UserSubscription subscription)
        {
            await _emContext.UserSubscriptions.AddAsync(subscription);
            await _emContext.SaveChangesAsync();
        }
        //V2 Implemented
        public async Task AddAsync(UserCreditLedgerEntry creditLedgerEntry)
        {
            await _emContext.UserCreditLedgerEntries.AddAsync(creditLedgerEntry);
            await _emContext.SaveChangesAsync();
        }
        //V2 Implemented
        public async Task AddAsync(List<UserCreditLedgerEntry> credsList)
        {
            await _emContext.UserCreditLedgerEntries.AddRangeAsync(credsList);
            await _emContext.SaveChangesAsync();
        }


    }
}
