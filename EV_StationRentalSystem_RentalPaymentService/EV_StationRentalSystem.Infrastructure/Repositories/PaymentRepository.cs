using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Infrastructure.Repositories
{
    public class PaymentRepository : IPaymentRepository
    {
        private readonly RentalPaymentDbContext _context;

        public PaymentRepository(RentalPaymentDbContext context)
        {
            _context = context;
        }

        public async Task<Payment> CreateAsync(Payment payment)
        {
            await _context.Payments.AddAsync(payment);
            await _context.SaveChangesAsync();
            return payment;
        }

        public async Task<Payment?> GetByIdAsync(Guid paymentId)
        {
            return await _context.Payments
                .Include(p => p.RentalOrder)
                .FirstOrDefaultAsync(p => p.PaymentId == paymentId);
        }

        public async Task<List<Payment>> GetAllAsync()
        {
            return await _context.Payments
                .OrderByDescending(p => p.PaymentTime)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByRentalIdAsync(Guid rentalId)
        {
            return await _context.Payments
                .Where(p => p.RentalId == rentalId)
                .OrderByDescending(p => p.PaymentTime)
                .ToListAsync();
        }

        public async Task<List<Payment>> GetByRenterIdAsync(Guid renterId)
        {
            return await _context.Payments
                .Include(p => p.RentalOrder)
                .Where(p => p.RentalOrder.RenterId == renterId)
                .OrderByDescending(p => p.PaymentTime)
                .ToListAsync();
        }

        public async Task<Payment> UpdateAsync(Payment payment)
        {
            _context.Payments.Update(payment);
            await _context.SaveChangesAsync();
            return payment;
        }
    }
}
