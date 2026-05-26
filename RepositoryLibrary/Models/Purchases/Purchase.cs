namespace RepositoryLibrary.Models
{
    public class Purchase
    {
        /// <summary>
        /// Internal identifier of the purchase request.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identity user identifier that owns the purchase.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// UTC date and time when the purchase request was created.
        /// </summary>
        public DateTime PurchasedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Current lifecycle state of the purchase request.
        /// </summary>
        public PurchaseStatus Status { get; set; } = PurchaseStatus.PendingPayment;

        /// <summary>
        /// Total amount to pay for all purchase lines.
        /// </summary>
        public decimal TotalAmount { get; set; }

        /// <summary>
        /// Monthly value of the selected subscription, when present.
        /// </summary>
        public decimal MonthlyRecurringAmount { get; set; }

        /// <summary>
        /// One-off amount from credit packs in this purchase.
        /// </summary>
        public decimal OneOffAmount { get; set; }

        /// <summary>
        /// Product lines included in this purchase request.
        /// </summary>
        public List<PurchaseLine> Lines { get; set; } = new();
    }
}
