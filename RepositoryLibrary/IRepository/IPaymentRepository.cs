using RepositoryLibrary.Models;

namespace RepositoryLibrary.IRepository
{
    public interface IPaymentRepository
    {
        Task<List<UserPayment>> GetPaymentsByUserId(string userId);
        Task<List<UserPayment>> GetPaymentsByMonth(DateOnly date);
        Task<List<UserPayment>> GetAllPayments();
        Task<UserPayment> CreatePayment(UserPayment payment);
        Task<UserPayment> DeletePayment(UserPayment payment);
        Task<UserPayment> EditPayment(UserPayment payment);
        Task<(bool weekly, int? amount)> IsWeekly(string userId);
        Task<string> LessonTypeBought(string userId);
        Task useClass(string userId);
    }
}
