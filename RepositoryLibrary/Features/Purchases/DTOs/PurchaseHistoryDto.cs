using RepositoryLibrary.Features.Purchases.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Purchases.DTOs
{
    public class PurchaseHistoryDto
    {
        public int PurchaseId { get; set; }
        public DateTime PurchasedAtUtc { get; set; }
        public PurchaseStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
