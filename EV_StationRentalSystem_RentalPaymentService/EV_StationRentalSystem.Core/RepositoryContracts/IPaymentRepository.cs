using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IPaymentRepository
    {
        Task<Payment> CreateAsync(Payment payment);
        Task<Payment?> GetByIdAsync(Guid paymentId);
        Task<Payment?> GetByTransactionCodeAsync(string transactionCode);
        Task<List<Payment>> GetAllAsync();
        Task<List<Payment>> GetByRentalIdAsync(Guid rentalId);
        Task<List<Payment>> GetByRenterIdAsync(Guid renterId);
        Task<Payment> UpdateAsync(Payment payment);
    }
}
