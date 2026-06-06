using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Purchases.DTOs
{
    public class PurchaseStudentSummary
    {
        public string StudentId { get; set; } = string.Empty;

        public string StudentName { get; set; } = string.Empty;

        public int? SchoolId { get; set; }

        public string? SchoolName { get; set; }

        public int PurchaseCount { get; set; }
    }
}
