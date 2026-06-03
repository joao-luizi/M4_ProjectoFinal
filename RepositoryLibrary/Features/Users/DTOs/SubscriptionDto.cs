using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Models.DTOs
{
    public class SubscriptionDto
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = "";
        public DateOnly PeriodStart { get; set; }
        public DateOnly PeriodEnd { get; set; }
        public SubscriptionStatus Status { get; set; }

        public List<EntitlementDto> Entitlements { get; set; } = new();
    }
}
