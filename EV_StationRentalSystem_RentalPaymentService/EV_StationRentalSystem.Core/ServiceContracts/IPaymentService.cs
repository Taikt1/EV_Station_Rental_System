using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.DTO.Response;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IPaymentService
    {
        Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request);
        Task<PaymentResponse?> GetPaymentByIdAsync(Guid paymentId);
        Task<PaymentResponse?> GetPaymentByTransactionCodeAsync(string transactionCode);
        Task<List<PaymentResponse>> GetAllPaymentsAsync();
        Task<List<PaymentResponse>> GetPaymentsByRentalIdAsync(Guid rentalId);
        Task<List<PaymentResponse>> GetPaymentsByRenterIdAsync(Guid renterId);
    }
}
