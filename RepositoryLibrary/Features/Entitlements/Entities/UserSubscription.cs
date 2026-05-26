using RepositoryLibrary.Features.Entitlements.Enums;
using RepositoryLibrary.Features.Products.Entities;
using RepositoryLibrary.Features.Purchases.Entities;

namespace RepositoryLibrary.Features.Entitlements.Entities
{
    public class UserSubscription
    {
        /// <summary>
        /// Internal identifier of the granted subscription.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identity user identifier that owns this subscription.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Subscription product that was bought.
        /// </summary>
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        /// <summary>
        /// Purchase line that generated this subscription.
        /// </summary>
        public int? PurchaseLineId { get; set; }
        public PurchaseLine? PurchaseLine { get; set; }

        /// <summary>
        /// Number of months paid for this subscription period.
        /// </summary>
        public int PurchasedMonths { get; set; }

        /// <summary>
        /// Inclusive first day when the subscription can be used.
        /// </summary>
        public DateOnly PeriodStart { get; set; }

        /// <summary>
        /// Inclusive last day when the subscription can be used.
        /// </summary>
        public DateOnly PeriodEnd { get; set; }

        /// <summary>
        /// Operational state used when checking subscription access.
        /// </summary>
        public SubscriptionStatus Status { get; set; } = SubscriptionStatus.Active;

        /// <summary>
        /// Weekly booking limits granted by this subscription.
        /// </summary>
        public List<UserSubscriptionEntitlement> Entitlements { get; set; } = new();
    }
}
