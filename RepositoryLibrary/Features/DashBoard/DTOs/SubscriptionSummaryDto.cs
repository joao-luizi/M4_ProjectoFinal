using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class SubscriptionSummaryDto
    {
        public int ActiveSubscriptionsCount { get; set; }

        public bool HasActiveSubscription => ActiveSubscriptionsCount > 0;
    }
}
