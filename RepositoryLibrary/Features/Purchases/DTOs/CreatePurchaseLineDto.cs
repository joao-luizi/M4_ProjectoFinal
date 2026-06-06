using RepositoryLibrary.Features.Purchases.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Purchases.DTOs
{
    public class CreatePurchaseLineDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public PurchaseLineKind Kind { get; set; }

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public int? SubscriptionMonths { get; set; }
    }
}
