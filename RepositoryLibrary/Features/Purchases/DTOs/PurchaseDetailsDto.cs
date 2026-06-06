using RepositoryLibrary.Features.Purchases.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Purchases.DTOs
{
    public class PurchaseDetailsDto
    {
        public int PurchaseId { get; set; }
        public DateTime PurchasedAtUtc { get; set; }
        public PurchaseStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal MonthlyRecurringAmount { get; set; }
        public decimal OneOffAmount { get; set; }

        public List<PurchaseDetailsLineDto> Lines { get; set; } = [];
    }
}
