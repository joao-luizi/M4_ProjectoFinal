using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.DashBoard.DTOs
{
    public class CreditByProductDto
    {
        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Credits { get; set; }
    }
}
