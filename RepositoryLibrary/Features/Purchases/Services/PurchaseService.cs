using Microsoft.Extensions.Logging;
using RepositoryLibrary.Features.Products.Interfaces;
using RepositoryLibrary.Features.Purchases.DTOs;
using RepositoryLibrary.Features.Purchases.Entities;
using RepositoryLibrary.Features.Purchases.Enums;
using RepositoryLibrary.Features.Purchases.Interfaces;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;


namespace RepositoryLibrary.Features.Purchases.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepo;
        private readonly ISchoolUsersRepository _schoolUsersRepo;
        private readonly IProductRepository _productRepo;
        private readonly ILogger<PurchaseService> _logger;

        public PurchaseService(
            IPurchaseRepository purchaseRepo,
            ISchoolUsersRepository schoolUsersRepo,
            ILogger<PurchaseService> logger)
        {
            _purchaseRepo = purchaseRepo;
            _schoolUsersRepo = schoolUsersRepo;
            _logger = logger;
        }

        //V2 implemented
        public async Task<List<PurchaseStudentSummary>> GetStudentPurchaseSummaryAsync()
        {
            _logger.LogInformation("A obter resumo de compras por estudante.");

            try
            {
                var students = await _schoolUsersRepo.GetAllWithIncludesAsync();

                var purchaseCounts = await _purchaseRepo.GetPurchaseCountsByUserAsync();

                var result = students
                    .Select(student =>
                    {
                        var count = purchaseCounts
                            .FirstOrDefault(x => x.UserId == student.UserId)
                            ?.Count ?? 0;

                        return new PurchaseStudentSummary
                        {
                            StudentId = student.UserId,
                            StudentName = $"{student.User.FirstName} {student.User.LastName}",
                            SchoolId = student.SchoolId,
                            SchoolName = student.School.SchoolName,
                            PurchaseCount = count
                        };
                    })
                    .OrderBy(x => x.StudentName)
                    .ToList();

                _logger.LogInformation(
                    "Resumo de compras gerado para {Count} estudantes.",
                    result.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter resumo de compras por estudante.");
                throw;
            }
        }

        //V2 implemented
        public async Task<List<PurchaseHistoryDto>> GetStudentPurchaseHistoryAsync(string studentId)
        {
            _logger.LogInformation(
                "A obter histórico de compras do estudante {StudentId}.",
                studentId);

            try
            {
                var purchases = await _purchaseRepo.GetByUserIdAsync(studentId);

                var result = purchases
                    .Select(p => new PurchaseHistoryDto
                    {
                        PurchaseId = p.Id,
                        PurchasedAtUtc = p.PurchasedAtUtc,
                        Status = p.Status,
                        TotalAmount = p.TotalAmount
                    })
                    .ToList();

                _logger.LogInformation(
                    "Obtidas {Count} compras para o estudante {StudentId}.",
                    result.Count,
                    studentId);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao obter histórico de compras do estudante {StudentId}.",
                    studentId);

                throw;
            }
        }

        //V2 implemented
        public async Task CreatePurchaseAsync(CreatePurchaseDto dto)
        {
            _logger.LogInformation(
                "A criar compra para utilizador {UserId}. PaymentReceived={PaymentReceived}",
                dto.UserId,
                dto.PaymentReceived);

            try
            {
                var productIds = dto.Lines
                    .Select(x => x.ProductId)
                    .Distinct()
                    .ToList();

                var products = await _productRepo.GetByIdsAsync(productIds);

                var purchase = new Purchase
                {
                    UserId = dto.UserId,
                    PurchasedAtUtc = DateTime.UtcNow,
                    Status = dto.PaymentReceived
                        ? PurchaseStatus.Paid
                        : PurchaseStatus.PendingPayment
                };

                foreach (var lineDto in dto.Lines)
                {
                    var product = products.First(x => x.Id == lineDto.ProductId);

                    var lineTotal = product.Price * lineDto.Quantity;

                    purchase.Lines.Add(new PurchaseLine
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Kind = lineDto.Kind,
                        Quantity = lineDto.Quantity,
                        UnitPrice = product.Price,
                        LineTotal = lineTotal,
                        SubscriptionMonths = lineDto.SubscriptionMonths
                    });

                    if (lineDto.Kind == PurchaseLineKind.Subscription)
                    {
                        purchase.MonthlyRecurringAmount += lineTotal;
                    }
                    else
                    {
                        purchase.OneOffAmount += lineTotal;
                    }
                }

                purchase.TotalAmount =
                    purchase.MonthlyRecurringAmount +
                    purchase.OneOffAmount;

                await _purchaseRepo.AddAsync(purchase);

                _logger.LogInformation(
                    "Compra criada com sucesso para utilizador {UserId}. Status={Status}",
                    dto.UserId,
                    purchase.Status);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Erro ao criar compra para utilizador {UserId}",
                    dto.UserId);

                throw;
            }
        }

    }

}