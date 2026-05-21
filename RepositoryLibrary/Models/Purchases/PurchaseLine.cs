namespace RepositoryLibrary.Models
{
    public class PurchaseLine
    {
        /// <summary>
        /// Internal identifier of this purchase line.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Purchase request that owns this line.
        /// </summary>
        public int PurchaseId { get; set; }
        public Purchase Purchase { get; set; } = null!;

        /// <summary>
        /// Product bought on this line.
        /// </summary>
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        /// <summary>
        /// Snapshot of the product name at purchase time.
        /// </summary>
        public string ProductName { get; set; } = null!;

        /// <summary>
        /// Distinguishes monthly subscriptions from credit packs.
        /// </summary>
        public PurchaseLineKind Kind { get; set; }

        /// <summary>
        /// Number of packs or subscription months bought.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// Product unit price at purchase time.
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// Total amount for this line.
        /// </summary>
        public decimal LineTotal { get; set; }

        /// <summary>
        /// Number of subscription months bought, only used for subscription lines.
        /// </summary>
        public int? SubscriptionMonths { get; set; }

        /// <summary>
        /// Inclusive first day covered by the subscription purchase.
        /// </summary>
        public DateOnly? SubscriptionPeriodStart { get; set; }

        /// <summary>
        /// Inclusive last day covered by the subscription purchase.
        /// </summary>
        public DateOnly? SubscriptionPeriodEnd { get; set; }
    }
}
