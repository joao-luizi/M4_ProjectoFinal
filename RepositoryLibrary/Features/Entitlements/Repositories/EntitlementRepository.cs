using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Entitlements.Entities;
using RepositoryLibrary.Features.Entitlements.Enums;
using RepositoryLibrary.Features.Entitlements.Interfaces;
using RepositoryLibrary.Features.Lessons.Entities;
using RepositoryLibrary.Features.Products.Entities;


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

        //V2 
        public async Task<int> GetWeeklyLimit(string userId, int lessonTypeId)
        {
            var entitlement = await _emContext.UserSubscriptionEntitlements
                .Where(x =>
                    x.LessonTypeId == lessonTypeId &&
                    x.UserSubscription.UserId == userId &&
                    x.UserSubscription.Status == SubscriptionStatus.Active)
                .Select(x => x.WeeklyFrequency)
                .FirstOrDefaultAsync();

            return entitlement;
        }

        //V2 Implemented
        public async Task<List<BookingValidationError>> GetSubscriptionErrorsAsync(
    string userId,
    int lessonTypeId)
        {
            var errors = new List<BookingValidationError>();

            var subscription = await _emContext.UserSubscriptionEntitlements
                .Where(x =>
                    x.LessonTypeId == lessonTypeId &&
                    x.UserSubscription.UserId == userId &&
                    x.UserSubscription.Status == SubscriptionStatus.Active)
                .Select(x => x.UserSubscription)
                .FirstOrDefaultAsync();

            if (subscription == null)
            {
                errors.Add(BookingValidationError.NoActiveSubscription);
                return errors;
            }

            var today = DateOnly.FromDateTime(DateTime.UtcNow);

            if (subscription.PeriodStart > today ||
                subscription.PeriodEnd < today)
            {
                errors.Add(BookingValidationError.SubscriptionExpired);
            }

            return errors;
        }

        public async Task<int> GetCreditBalanceAsync(string userId, int lessonTypeId)
        {
            var now = DateTime.UtcNow;

            return await _emContext.UserCreditLedgerEntries
                .Where(x =>
                    x.UserId == userId &&
                    x.LessonTypeId == lessonTypeId &&
                    (x.ExpiresAtUtc == null || x.ExpiresAtUtc >= now))
                .SumAsync(x => x.CreditsDelta);
        }

        public async Task AddCreditLedgerEntryAsync(UserCreditLedgerEntry entry)
        {
            await _emContext.UserCreditLedgerEntries.AddAsync(entry);
            await _emContext.SaveChangesAsync();
        }


    }
}
