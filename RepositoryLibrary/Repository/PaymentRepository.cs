using System;
using RepositoryLibrary.IRepository;
using RepositoryLibrary.Models;
using RepositoryLibrary.Models.Context;
using Microsoft.EntityFrameworkCore;

namespace RepositoryLibrary.Repository;

public class PaymentRepository : IPaymentRepository
{
    private readonly EM_DbContext _dbContext;
    public PaymentRepository(EM_DbContext context)
    {
        _dbContext = context;
    }
    public async Task<List<UserPayment>> GetPaymentsByUserId(string userId)
    {
        var payments = await _dbContext.UserPayments
            .Where(p => p.UserId == userId)
            .ToListAsync();

        if (payments.Count == 0)
            throw new Exception("No User Payments found for this User.");

        return payments;
    }

    public async Task<List<UserPayment>> GetAllPayments()
    {
        var payments = await _dbContext.UserPayments.ToListAsync();

        if (payments.Count == 0) throw new Exception("No payments found.");

        return payments;
    }

    public async Task<List<UserPayment>> GetPaymentsByMonth(DateOnly date)
    {
        var from = new DateOnly(date.Year, date.Month, 1);
        var to = from.AddMonths(1);

        var payments = await _dbContext.UserPayments
        .Where(p => p.BuyDate >= from && p.BuyDate < to)
        .ToListAsync();

        if (payments.Count == 0) throw new Exception("No payments found for this month.");

        return payments;
    }

    public async Task<UserPayment> CreatePayment(UserPayment payment)
    {
        if (payment == null) throw new Exception("payment is null");
        await _dbContext.UserPayments.AddAsync(payment);
        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            throw new Exception("Failed to add payment", e);
        }
        return payment;
    }

    public async Task<UserPayment> DeletePayment(UserPayment payment)
    {

        var existing = await _dbContext.UserPayments
            .FirstOrDefaultAsync(up => up.UserId == payment.UserId
                                && up.BuyDate == payment.BuyDate);
        if (existing == null)
            throw new KeyNotFoundException("No payment found to delete.");

        _dbContext.UserPayments.Remove(existing);
        await _dbContext.SaveChangesAsync();
        return existing;
    }

    public async Task<UserPayment> EditPayment(UserPayment payment)
    {
        var existing = await _dbContext.UserPayments
      .FirstOrDefaultAsync(up => up.UserId == payment.UserId
                              && up.BuyDate == payment.BuyDate);
        if (existing == null)
            throw new Exception("No payment found to update.");

        _dbContext.Entry(existing).CurrentValues.SetValues(payment);

        try
        {
            await _dbContext.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            throw new InvalidOperationException("Concurrent update failed.", ex);
        }

        return existing;
    }

    public async Task<(bool weekly, int? amount)> IsWeekly(string userId)
    {
        var payment = await _dbContext.UserPayments.Include(p => p.PackageBought).OrderByDescending(p => p.BuyDate).FirstOrDefaultAsync(p => p.UserId == userId);
        if (payment.PackageBought.Weekly) return (true, payment.PackageBought.ClassesIncluded);
        return (false, payment.AmountOfClasses);
    }

    public async Task<string> LessonTypeBought(string userId)
    {
        var payment = await _dbContext.UserPayments
                    .Include(p => p.PackageBought).ThenInclude(p => p.LessonType)
                    .OrderByDescending(up => up.BuyDate)
                    .FirstOrDefaultAsync(up => up.UserId == userId);
        return payment.PackageBought.LessonType.Name ?? "erro";
    }
    public async Task useClass(string userId)
    {
        var up = await _dbContext.UserPayments.FirstOrDefaultAsync(up => up.UserId == userId);
        up.AmountOfClasses -= 1;
        _dbContext.UserPayments.Update(up);
        await _dbContext.SaveChangesAsync();
    }
}
