using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Purchases.DTOs
{
    public class CreatePurchaseDto
    {
        public string UserId { get; set; } = string.Empty;

        public bool PaymentReceived { get; set; }

        public List<CreatePurchaseLineDto> Lines { get; set; } = [];
    }
}
