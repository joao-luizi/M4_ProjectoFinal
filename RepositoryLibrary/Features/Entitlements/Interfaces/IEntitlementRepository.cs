using RepositoryLibrary.Features.Entitlements.Entities;
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
    }
}
