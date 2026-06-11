using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Entitlements.Enums
{
    public enum BookingValidationError
    {
        LessonNotFound,
        AlreadyBooked,
        NoActiveSubscription,
        SubscriptionExpired,
        NoCreditsAvailable,
        SubscriptionLimitExceededUsingCredit,
        WeeklyLimitExceeded
    }
}
