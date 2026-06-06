using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class CreditSummaryDto
    {
        public int TotalCredits { get; set; }

        public List<CreditByProductDto> ByProduct { get; set; } = new();
    }
}
