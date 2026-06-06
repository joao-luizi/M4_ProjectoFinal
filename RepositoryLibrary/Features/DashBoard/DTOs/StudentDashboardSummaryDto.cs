using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class StudentDashboardSummaryDto
    {
        public int ScheduledClassesCount { get; set; }

        public DateTime? NextClassDate { get; set; }

        public SubscriptionSummaryDto Subscription { get; set; } = new();

        public CreditSummaryDto Credits { get; set; } = new();
    }
}
