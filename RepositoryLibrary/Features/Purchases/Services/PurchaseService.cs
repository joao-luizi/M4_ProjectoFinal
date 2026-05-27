using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.Data.Context;
using RepositoryLibrary.Features.Entitlements;
using RepositoryLibrary.Features.Entitlements.Entities;
using RepositoryLibrary.Features.Entitlements.Enums;
using RepositoryLibrary.Features.Purchases.Entities;
using RepositoryLibrary.Features.Purchases.Enums;
using RepositoryLibrary.Features.Purchases.Interfaces;
using RepositoryLibrary.Features.Purchases.Snapshot;


namespace RepositoryLibrary.Features.Purchases.Services
{
    public class PurchaseService : IPurchaseService
    {
        private readonly RideReadyDbContext _context;
        public PurchaseService(RideReadyDbContext context)
        {
            _context = context;
        }


        /// <summary>
        /// Creates a pending purchase request and keeps entitlement provisioning blocked.
        /// </summary>
        public async Task<Purchase> CreatePendingPurchase(string userId, IReadOnlyCollection<PurchaseRequestLine> lines)
        {
            if (!lines.Any())
            {
                throw new InvalidOperationException("A purchase must have at least one line.");
            }

            var purchase = new Purchase
            {
                UserId = userId,
                PurchasedAtUtc = DateTime.UtcNow,
                Status = PurchaseStatus.PendingPayment
            };

            foreach (var requestLine in lines)
            {
                var unitPrice = requestLine.Product.Price;
                var lineTotal = unitPrice * requestLine.Quantity;

                purchase.Lines.Add(new PurchaseLine
                {
                    ProductId = requestLine.Product.Id,
                    //Product = requestLine.Product,
                    ProductName = requestLine.Product.Name,
                    Kind = requestLine.Kind,
                    Quantity = requestLine.Quantity,
                    UnitPrice = unitPrice,
                    LineTotal = lineTotal,
                    SubscriptionMonths = requestLine.Kind == PurchaseLineKind.Subscription ? requestLine.Quantity : null,
                    SubscriptionPeriodStart = requestLine.SubscriptionPeriodStart,
                    SubscriptionPeriodEnd = requestLine.SubscriptionPeriodEnd
                });

                if (requestLine.Kind == PurchaseLineKind.Subscription)
                {
                    purchase.MonthlyRecurringAmount += lineTotal;
                }
                else
                {
                    purchase.OneOffAmount += lineTotal;
                }

                purchase.TotalAmount += lineTotal;
            }

            _context.Purchases.Add(purchase);

            await _context.SaveChangesAsync();
            return purchase;

        }

        /// <summary>
        /// Confirms payment and provisions subscriptions or credits exactly once.
        /// </summary>
        public async Task<Purchase?> ConfirmPayment(int purchaseId)
        {
            var purchase = await _context.Purchases
                .Include(x => x.Lines)
                .ThenInclude(l => l.Product)
                .ThenInclude(p => p.Entitlements)
                .ThenInclude(e => e.LessonType)
                .FirstOrDefaultAsync(x => x.Id == purchaseId);
            //var purchase = _purchases.FirstOrDefault(p => p.Id == purchaseId);

            if (purchase is null)
                return null;

            if (purchase.Status == PurchaseStatus.Paid)
                return purchase;

            if (purchase.Status != PurchaseStatus.PendingPayment)
            {
                throw new InvalidOperationException($"Purchase {purchase.Id} cannot be paid from status {purchase.Status}.");
            }

            purchase.Status = PurchaseStatus.Paid;
            ProvisionEntitlements(purchase);

            await _context.SaveChangesAsync();

            return purchase;
            //return Task.FromResult<Purchase?>(purchase);
        }

        /// <summary>
        /// Cancels an unpaid purchase request.
        /// </summary>
        public async Task<Purchase?> CancelPurchase(int purchaseId)
        {
            //var purchase = _purchases.FirstOrDefault(p => p.Id == purchaseId);
            var purchase = await _context.Purchases
               .Include(x => x.Lines)
               .ThenInclude(l => l.Product)
               .ThenInclude(p => p.Entitlements)
               .ThenInclude(e => e.LessonType)
               .FirstOrDefaultAsync(x => x.Id == purchaseId);
            if (purchase is null)
            {
                //return Task.FromResult<Purchase?>(null);
                return null;
            }

            if (purchase.Status == PurchaseStatus.Paid)
            {
                throw new InvalidOperationException($"Paid purchase {purchase.Id} cannot be cancelled.");
            }

            purchase.Status = PurchaseStatus.Cancelled;
            //return Task.FromResult<Purchase?>(purchase);
            await _context.SaveChangesAsync();

            return purchase;
        }

        /// <summary>
        /// Lists all purchase requests created by a user.
        /// </summary>
        public async Task<List<Purchase>> GetPurchasesForUser(string userId)
        {
            return await _context.Purchases
              .Include(x => x.Lines)
              .ThenInclude(l => l.Product)
              .ThenInclude(p => p.Entitlements)
              .ThenInclude(e => e.LessonType)
              .Where(x => x.UserId == userId)
              .ToListAsync();
        }


        /// <summary>
        /// Returns the currently provisioned in-memory entitlements for a user.
        /// </summary>
        public async Task<PurchaseEntitlementSnapshot> GetEntitlementsForUser(string userId)
        {
            var userSubscription = await _context.UserSubscriptions.Where(s => s.UserId == userId).ToListAsync();
            var userCreditsLedger = await _context.UserCreditLedgerEntries.Where(s => s.UserId == userId).ToListAsync();

            return new PurchaseEntitlementSnapshot(userSubscription, userCreditsLedger);
        }
        

        /// <summary>
        /// Routes each paid purchase line to the matching entitlement provisioning path.
        /// </summary>
        private void ProvisionEntitlements(Purchase purchase)
        {
            foreach (var line in purchase.Lines)
            {

                switch (line.Kind)
                {
                    case PurchaseLineKind.Subscription:
                        ProvisionSubscription(purchase, line);
                        break;

                    case PurchaseLineKind.CreditPack:
                        ProvisionCredits(purchase, line);
                        break;

                    default:
                        throw new InvalidOperationException("O tipo de Linha não é reconhecido.");
                }

            }
        }

        /// <summary>
        /// Creates the active subscription and weekly lesson limits from a paid subscription line.
        /// </summary>
        private void ProvisionSubscription(Purchase purchase, PurchaseLine line)
        {
            if (line.SubscriptionPeriodStart is null || line.SubscriptionPeriodEnd is null)
            {
                throw new InvalidOperationException($"Subscription line {line.Id} is missing a period.");
            }

            var subscription = new UserSubscription
            {
                UserId = purchase.UserId,
                PurchaseLineId = line.Id,
                ProductId = line.ProductId,
                PurchasedMonths = line.SubscriptionMonths ?? line.Quantity,
                PeriodStart = line.SubscriptionPeriodStart.Value,
                PeriodEnd = line.SubscriptionPeriodEnd.Value,
                Status = SubscriptionStatus.Active
            };

            var productEntitlements = _context.ProductEntitlements
            .Where(e => e.ProductId == line.ProductId)
            .Include(e => e.LessonType)
            .ToList();

            foreach (var entitlement in productEntitlements.Where(e => (e.WeeklyFrequency ?? 0) > 0))
            {
                subscription.Entitlements.Add(new UserSubscriptionEntitlement
                {
                    LessonTypeId = entitlement.LessonTypeId,
                    WeeklyFrequency = entitlement.WeeklyFrequency!.Value
                });
            }

            //_subscriptions.Add(subscription);

            _context.UserSubscriptions.Add(subscription);

        }

        /// <summary>
        /// Creates positive credit ledger entries from a paid credit pack line.
        /// </summary>
        private void ProvisionCredits(Purchase purchase, PurchaseLine line)
        {
            var creditLedgerEntries = new List<UserCreditLedgerEntry>();

            var productEntitlements = _context.ProductEntitlements
           .Where(e => e.ProductId == line.ProductId)
           .Include(e => e.LessonType)
           .ToList();

            foreach (var entitlement in productEntitlements.Where(e => (e.CreditsGranted ?? 0) > 0))
            {
                creditLedgerEntries.Add(new UserCreditLedgerEntry
                {
                    UserId = purchase.UserId,
                    ProductId = line.ProductId,
                    PurchaseLineId = line.Id,
                    LessonTypeId = entitlement.LessonTypeId,
                    CreditsDelta = entitlement.CreditsGranted!.Value * line.Quantity,
                    CreatedAtUtc = DateTime.UtcNow,
                    Reason = $"Purchase {purchase.Id}: {line.ProductName}"
                });
            }
            _context.UserCreditLedgerEntries.AddRange(creditLedgerEntries);

        }

        public async Task<List<Purchase>> GetPurchasesByUserId(string userId)
        {
            return await _context.Purchases
                .Where(x => x.UserId == userId)
                .Include(x => x.Lines)
                    .ThenInclude(l => l.Product)
                .AsNoTracking()
                .ToListAsync();
        }
    }

}