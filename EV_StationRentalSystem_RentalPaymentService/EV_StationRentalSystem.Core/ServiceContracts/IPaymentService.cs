using EV_StationRentalSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
        Task<PaymentResponse?> GetPaymentByIdAsync(Guid paymentId);
        Task<List<PaymentResponse>> GetAllPaymentsAsync();
        Task<List<PaymentResponse>> GetPaymentsByRentalIdAsync(Guid rentalId);
        Task<List<PaymentResponse>> GetPaymentsByRenterIdAsync(Guid renterId);
    }
}
