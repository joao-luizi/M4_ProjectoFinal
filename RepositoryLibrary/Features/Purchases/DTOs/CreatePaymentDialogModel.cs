using RepositoryLibrary.Features.Purchases.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Purchases.DTOs
{
    public class CreatePaymentDialogModel
    {
        public string UserId { get; set; }

        public List<PurchaseRequestLine> Lines { get; set; }

        public bool PaymentReceived { get; set; }
    }
}
