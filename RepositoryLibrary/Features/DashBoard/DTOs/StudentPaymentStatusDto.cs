using RepositoryLibrary.Features.Purchases.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class StudentPaymentStatusDto
    {
        public PurchaseStatus Status { get; set; }

        public DateTime? LastPaymentDate { get; set; }

        public DateTime? NextDueDate { get; set; }

        public decimal? OutstandingAmount { get; set; }
    }
}
