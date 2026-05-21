namespace RepositoryLibrary.Models
{
    public class UserCreditLedgerEntry
    {
        /// <summary>
        /// Internal identifier of the credit ledger movement.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Identity user identifier that owns this credit movement.
        /// </summary>
        public string UserId { get; set; } = null!;

        /// <summary>
        /// Lesson type whose credit balance is affected.
        /// </summary>
        public int LessonTypeId { get; set; }
        public LessonType LessonType { get; set; } = null!;

        /// <summary>
        /// Product that generated the credit movement, when applicable.
        /// </summary>
        public int? ProductId { get; set; }
        public Product? Product { get; set; }

        /// <summary>
        /// Purchase line that generated the credit movement, when applicable.
        /// </summary>
        public int? PurchaseLineId { get; set; }
        public PurchaseLine? PurchaseLine { get; set; }

        /// <summary>
        /// Positive or negative credit movement for this lesson type.
        /// </summary>
        public int CreditsDelta { get; set; }

        /// <summary>
        /// UTC date and time when the credit movement was created.
        /// </summary>
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Optional date after which these credits should no longer be usable.
        /// </summary>
        public DateTime? ExpiresAtUtc { get; set; }

        /// <summary>
        /// Human-readable reason for this ledger movement.
        /// </summary>
        public string Reason { get; set; } = null!;
    }
}
