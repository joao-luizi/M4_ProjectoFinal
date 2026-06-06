using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Purchases.DTOs
{
    public class PurchaseCountByUser
    {
        public string UserId { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}
