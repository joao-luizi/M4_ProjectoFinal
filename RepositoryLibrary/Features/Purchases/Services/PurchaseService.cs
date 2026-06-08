using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml;
using Microsoft.Extensions.Logging;
using RepositoryLibrary.Features.Entitlements.Entities;
using RepositoryLibrary.Features.Entitlements.Enums;
using RepositoryLibrary.Features.Entitlements.Interfaces;
using RepositoryLibrary.Features.Products.Entities;
using RepositoryLibrary.Features.Products.Interfaces;
using RepositoryLibrary.Features.Purchases.DTOs;
using RepositoryLibrary.Features.Purchases.Entities;
using RepositoryLibrary.Features.Purchases.Enums;
using RepositoryLibrary.Features.Purchases.Interfaces;
using RepositoryLibrary.Features.Users.Entities;
using RepositoryLibrary.Features.Users.Interfaces;
using System.Linq;


namespace RepositoryLibrary.Features.Purchases.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly IPurchaseRepository _purchaseRepo;
        private readonly IUserRepository _userRepo;
        private readonly ISchoolUsersRepository _schoolUsersRepo;
        private readonly IProductRepository _productRepo;
        private readonly IEntitlementRepository _entitlementRepo;
        private readonly ILogger<PurchaseService> _logger;

        public PurchaseService(
            IPurchaseRepository purchaseRepo,
            ISchoolUsersRepository schoolUsersRepo,
             IProductRepository productRepo,
            IUserRepository userRepo,
            IEntitlementRepository entitlementRepo,
        ILogger<PurchaseService> logger)
        {
            _purchaseRepo = purchaseRepo;
            _schoolUsersRepo = schoolUsersRepo;
            _productRepo = productRepo;
            _userRepo = userRepo;
            _entitlementRepo = entitlementRepo;
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

                var userIds = students
                .Select(x => x.UserId)
                .Distinct()
                .ToList();

                var emUsers = await  _userRepo.GetUsersByIdsAsync(userIds);

                var userDict = emUsers.ToDictionary(x => x.Id);

                var result = students
                .Select(student =>
                {
                    userDict.TryGetValue(student.UserId, out var user);

                    var count = purchaseCounts
                        .FirstOrDefault(x => x.UserId == student.UserId)
                        ?.Count ?? 0;

                    return new PurchaseStudentSummary
                    {
                        StudentId = student.UserId,
                        StudentName = user != null
                            ? $"{user.FirstName} {user.LastName} ({user.Email})"
                            : "Unknown",

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
                        TotalAmount = p.TotalAmount,

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

                    decimal lineTotal = 0;

                    var purchaseLine = new PurchaseLine
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        Kind = lineDto.Kind,
                        UnitPrice = product.Price,
                        SubscriptionMonths = lineDto.Months
                    };

                    // =========================
                    // SUBSCRIPTION
                    // =========================
                    if (lineDto.Kind == PurchaseLineKind.Subscription)
                    {
                        var months = lineDto.Months ?? 0;

                        lineTotal = product.Price * months;

                        purchaseLine.Quantity = 1; // semantic only (1 subscription line)
                        purchaseLine.LineTotal = lineTotal;

                        purchase.MonthlyRecurringAmount += lineTotal;
                    }

                    // =========================
                    // CREDIT PACK
                    // =========================
                    else if (lineDto.Kind == PurchaseLineKind.CreditPack)
                    {
                        var quantity = lineDto.Quantity ?? 0;

                        lineTotal = product.Price * quantity;

                        purchaseLine.Quantity = quantity;
                        purchaseLine.LineTotal = lineTotal;

                        purchase.OneOffAmount += lineTotal;
                    }

                    purchase.Lines.Add(purchaseLine);
                }

                purchase.TotalAmount =
                    purchase.MonthlyRecurringAmount +
                    purchase.OneOffAmount;

                await _purchaseRepo.AddAsync(purchase);

                if (purchase.Status == PurchaseStatus.Paid)
                {
                    await GrantPurchaseEntitlementsAsync(
                        dto.UserId,
                        purchase,
                        products);
                }

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

        private async Task GrantPurchaseEntitlementsAsync(string userId,
            Purchase purchase, List<Product> products)
        {
                    var start = new DateOnly(
                    DateTime.Today.Year,
                    DateTime.Today.Month,
                    1);
            foreach(var line in purchase.Lines)
            {
                if (line.Kind == PurchaseLineKind.Subscription)
                {
                    var product = products.First(x => x.Id == line.ProductId);

                    var subscription = new UserSubscription
                    {
                        UserId = userId,
                        ProductId = product.Id,
                        PurchaseLineId = line.Id,

                        PurchasedMonths = line.SubscriptionMonths ?? 0,

                        PeriodStart = line.SubscriptionPeriodStart ?? start,
                        PeriodEnd = line.SubscriptionPeriodEnd ?? start.AddMonths(line.SubscriptionMonths ?? 0),

                        Status = SubscriptionStatus.Active,

                        Entitlements = new()
                    };

                    foreach (var entitlement in product.Entitlements)
                    {
                        if (entitlement.WeeklyFrequency is null)
                            continue;

                        subscription.Entitlements.Add(
                            new UserSubscriptionEntitlement
                            {
                                LessonTypeId = entitlement.LessonTypeId,
                                WeeklyFrequency = entitlement.WeeklyFrequency.Value
                            });
                    }

                    await _entitlementRepo.AddAsync(subscription);

                }
                if (line.Kind == PurchaseLineKind.CreditPack)
                {
                    var product = products.First(x => x.Id == line.ProductId);

                    List<UserCreditLedgerEntry> userCreds = new();
                    foreach (var entitlement in product.Entitlements)
                    {
                        var credits =
                        entitlement.CreditsGranted.GetValueOrDefault() * line.Quantity;
                        userCreds.Add(new UserCreditLedgerEntry {
                            UserId = userId,
                            LessonTypeId = entitlement.LessonTypeId,

                            ProductId = product.Id,
                            PurchaseLineId = line.Id,

                            CreditsDelta = credits,

                            Reason = $"Purchase #{purchase.Id}"
                        });
                    }
                    if (userCreds.Any())
                    {
                        await _entitlementRepo.AddAsync(userCreds);
                    }
                }

            }

        }
        
    }

}