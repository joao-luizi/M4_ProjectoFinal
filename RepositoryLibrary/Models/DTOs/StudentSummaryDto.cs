using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Models.DTOs
{
    public class StudentSummaryDto
    {
        public string StudentName { get; set; } = "";
        public string StudentEmail { get; set; } = "";

        public List<SubscriptionDto> ActiveSubscriptions { get; set; } = new();
        public List<CreditBalanceDto> Credits { get; set; } = new();

        public decimal TotalSpent { get; set; }
    }
}
