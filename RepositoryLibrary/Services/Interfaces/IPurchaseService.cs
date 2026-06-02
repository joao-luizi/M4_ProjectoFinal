

using RepositoryLibrary.Models;

namespace RepositoryLibrary.Services
{
    public interface IPurchaseService
    {

        Task<Purchase?> GetByPurchaseId(int purchaseId);
        /// <summary>
        /// Creates a purchase intent without granting subscriptions or credits.
        /// </summary>
       Task<Purchase> CreatePendingPurchase(string userId, IReadOnlyCollection<PurchaseRequestLine> lines);

        /// <summary>
        /// Marks a pending purchase as paid and provisions the purchased entitlements.
        /// </summary>
        Task<Purchase?> ConfirmPayment(int purchaseId);

        /// <summary>
        /// Cancels a purchase request that has not been paid yet.
        /// </summary>
        Task<Purchase?> CancelPurchase(int purchaseId);

        /// <summary>
        /// Returns the purchase requests created for a specific user.
        /// </summary>
        Task<List<Purchase>> GetPurchasesForUser(string userId);

        /// <summary>
        /// Returns the in-memory subscriptions and credit movements already granted to a user.
        /// </summary>
        Task<PurchaseEntitlementSnapshot> GetEntitlementsForUser(string userId);

        Task<List<Purchase>> GetPurchasesByUserId(string userId);
    }
}
