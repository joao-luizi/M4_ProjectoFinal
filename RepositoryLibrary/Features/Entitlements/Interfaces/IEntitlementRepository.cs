using RepositoryLibrary.Features.Entitlements.Entities;
using RepositoryLibrary.Features.Entitlements.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Entitlements.Interfaces
{
    public interface IEntitlementRepository
    {
        //V2 Implemented
        Task<List<UserCreditLedgerEntry>> GetCreditLedgerAsync(string userId);
        //V2 Implemented
        Task<List<UserSubscriptionEntitlement>> GetSubscriptionEntitlementsAsync(string userId);

        //V2 Implemented
        Task AddAsync(UserSubscription subscription);
        //V2 Implemented
        Task AddAsync(UserCreditLedgerEntry creditLedgerEntry);
        //V2 Implemented
        Task AddAsync(List<UserCreditLedgerEntry> credsList);

        //V2 implemented
        Task<List<BookingValidationError>> GetSubscriptionErrorsAsync(string userId, int lessonTypeId);

        //V2 implemented
        Task<int> GetCreditBalanceAsync(string userId, int lessonTypeId);

        Task<int> GetWeeklyLimit(string userId, int lessonTypeId);




    }
}
