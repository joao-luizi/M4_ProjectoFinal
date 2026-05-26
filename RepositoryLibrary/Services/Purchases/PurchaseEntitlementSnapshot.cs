

using RepositoryLibrary.Models;

namespace RepositoryLibrary.Services
{
    /// <summary>
    /// Read model with the entitlements already provisioned for a user.
    /// </summary>
    /// <param name="Subscriptions">Active or historical subscriptions granted to the user.</param>
    /// <param name="CreditLedgerEntries">Credit ledger movements granted to the user.</param>
    public sealed record PurchaseEntitlementSnapshot(
        List<UserSubscription> Subscriptions,
        List<UserCreditLedgerEntry> CreditLedgerEntries);
}
