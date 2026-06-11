using RepositoryLibrary.Features.Entitlements.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Features.Entitlements.DTOs
{
    public class BookingResult
    {
        public bool Success { get; set; }

        public List<BookingValidationError> Errors { get; set; } = new();
    }
}
