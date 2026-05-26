using RepositoryLibrary.Features.Products.Entities;

namespace RepositoryLibrary.Features.Entitlements.Entities
{
    public class UserSubscriptionEntitlement
    {
        /// <summary>
        /// Internal identifier of the subscription entitlement.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Subscription that owns this weekly limit.
        /// </summary>
        public int UserSubscriptionId { get; set; }
        public UserSubscription UserSubscription { get; set; } = null!;

        /// <summary>
        /// Lesson type covered by this weekly entitlement.
        /// </summary>
        public int LessonTypeId { get; set; }
        public LessonType LessonType { get; set; } = null!;

        /// <summary>
        /// Maximum number of bookings allowed per week for this lesson type.
        /// </summary>
        public int WeeklyFrequency { get; set; }
    }
}
