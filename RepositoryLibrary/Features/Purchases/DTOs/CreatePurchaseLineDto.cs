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
        public decimal UnitPrice { get; set; }

        public PurchaseLineKind Kind { get; set; }

        // Credit
        public int? Quantity { get; set; }

        // Subscription
        public int? Months { get; set; }
    }
}
