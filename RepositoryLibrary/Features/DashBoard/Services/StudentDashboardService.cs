using RepositoryLibrary.Features.Bookings.Interfaces;
using RepositoryLibrary.Features.DashBoard.DTOs;
using RepositoryLibrary.Features.DashBoard.Interfaces;
using RepositoryLibrary.Features.Entitlements.Enums;
using RepositoryLibrary.Features.Entitlements.Interfaces;
using RepositoryLibrary.Features.Purchases.Enums;
using RepositoryLibrary.Features.Purchases.Interfaces;


namespace RepositoryLibrary.Features.DashBoard.Services
{
    public class StudentDashboardService : IStudentDashboardService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly IPurchaseRepository _purchaseRepo;
        private readonly IEntitlementRepository _entitlementRepo;
        

        public StudentDashboardService(
            IBookingRepository bookingRepo,
            IPurchaseRepository purchaseRepos,
            IEntitlementRepository entitlementRepo)
        {
            _bookingRepo = bookingRepo;
            _purchaseRepo = purchaseRepos;
            _entitlementRepo = entitlementRepo;
           
        }

        //V2 Implemented
        public async Task<StudentDashboardDto> GetStudentDashboardAsync(string studentId)
        {
            var now = DateTime.UtcNow;

            var bookingsTask = _bookingRepo.GetBookingsByUserIdAsync(studentId);
            var subscriptionsTask = _entitlementRepo.GetSubscriptionEntitlementsAsync(studentId);
            var creditsTask = _entitlementRepo.GetCreditLedgerAsync(studentId);

            await Task.WhenAll(bookingsTask, subscriptionsTask, creditsTask);

            var bookings = await bookingsTask;
            var subscriptions = await subscriptionsTask;
            var creditEntries = await creditsTask;

            // -------------------------
            // BOOKINGS
            // -------------------------
            var upcoming = bookings
                .Where(b => b.Lesson.BeginOfLesson > now)
                .OrderBy(b => b.Lesson.BeginOfLesson)
                .ToList();

            // -------------------------
            // SUBSCRIPTIONS
            // -------------------------
            var activeSubscriptions = subscriptions
                .Where(s => s.UserSubscription.Status == SubscriptionStatus.Active)
                .ToList();

            // -------------------------
            // CREDITS
            // -------------------------
            var creditsByProduct = creditEntries
                .GroupBy(x => x.ProductId)
                .Select(g => new CreditByProductDto
                {
                    ProductId = g.Key ?? 0,
                    ProductName = g.First().Product?.Name ?? "Unknown",
                    Credits = g.Sum(x => x.CreditsDelta)
                })
                .ToList();

            var totalCredits = creditsByProduct.Sum(x => x.Credits);

            // -------------------------
            // RESPONSE
            // -------------------------
            return new StudentDashboardDto
            {
                Summary = new StudentDashboardSummaryDto
                {
                    ScheduledClassesCount = bookings.Count,

                    NextClassDate = upcoming.FirstOrDefault()?.Lesson.BeginOfLesson,

                    Subscription = new SubscriptionSummaryDto
                    {
                        ActiveSubscriptionsCount = activeSubscriptions.Count
                    },

                    Credits = new CreditSummaryDto
                    {
                        TotalCredits = totalCredits,
                        ByProduct = creditsByProduct
                    }
                }
            };
        }
    }
}
