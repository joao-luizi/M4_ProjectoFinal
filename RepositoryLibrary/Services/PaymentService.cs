using RepositoryLibrary.IRepository;
using RepositoryLibrary.IServices;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositoryLibrary.Services
{
    public  class PaymentService :IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository PaymentRepository) 
        { 
            _paymentRepository = PaymentRepository;

        }

        public async Task<int> CreateUserPayments(
            string userId,
            List<PaymentPreviewLines> lines)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var payments = lines.Select(x => new UserPayment
            {
                UserId = userId,
                PackageId = x.PackageId,

                BuyDate = today,

                // temporário
                DueDate = today.AddMonths(1),

                AmountOfClasses = x.ClassesIncluded * x.Quantity
            }).ToList();

            return await _paymentRepository.CreateAllPayments(payments);
        }
    }
}
