using RepositoryLibrary.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Models.DTOs
{
    public class CreatePaymentDialogModel
    {
        public string UserId { get; set; }

        public List<PurchaseRequestLine> Lines { get; set; }

        public bool PaymentReceived { get; set; }
    }
}
