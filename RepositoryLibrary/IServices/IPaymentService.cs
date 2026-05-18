using RepositoryLibrary.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.IServices
{
    public interface IPaymentService
    {
        Task<int> CreateUserPayments(string userId, List<PaymentPreviewLines> lines);
    }
}
