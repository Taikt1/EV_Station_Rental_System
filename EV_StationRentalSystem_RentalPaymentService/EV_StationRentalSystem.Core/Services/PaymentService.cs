using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.DTO.Response;
using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _paymentRepository;

        public PaymentService(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentResponse> CreatePaymentAsync(CreatePaymentRequest request)
        {
            // Validate and parse RentalId
            if (!Guid.TryParse(request.RentalId, out var rentalId))
                throw new ArgumentException("Invalid RentalId format");

            string paymentStatus = "Pending";
            if (request.TransactionRef != null && request.TransactionRef.Contains("DEPOSIT"))
            {
                paymentStatus = "Paid"; 
            }

            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                RentalId = rentalId,
                Amount = request.Amount,
                PaymentMethod = request.PaymentMethod,
                PaymentTime = DateTime.UtcNow,
                Status = paymentStatus,
                TransactionRef = request.TransactionRef
            };

            var createdPayment = await _paymentRepository.CreateAsync(payment);

            return new PaymentResponse
            {
                PaymentId = createdPayment.PaymentId.ToString(),
                RentalId = createdPayment.RentalId.ToString(),
                PaymentMethod = createdPayment.PaymentMethod ?? string.Empty,
                Amount = createdPayment.Amount,
                PaymentTime = createdPayment.PaymentTime,
                Status = createdPayment.Status ?? string.Empty,
                TransactionRef = createdPayment.TransactionRef
            };
        }

        public async Task<List<PaymentResponse>> GetAllPaymentsAsync()
        {
            var payments = await _paymentRepository.GetAllAsync();

            return payments.Select(p => new PaymentResponse
            {
                PaymentId = p.PaymentId.ToString(),
                RentalId = p.RentalId.ToString(),
                PaymentMethod = p.PaymentMethod ?? string.Empty,
                Amount = p.Amount,
                PaymentTime = p.PaymentTime,
                Status = p.Status ?? string.Empty,
                TransactionRef = p.TransactionRef
            }).ToList();
        }

        public async Task<PaymentResponse?> GetPaymentByIdAsync(Guid paymentId)
        {
            var payment = await _paymentRepository.GetByIdAsync(paymentId);
            
            if (payment == null)
                return null;

            return new PaymentResponse
            {
                PaymentId = payment.PaymentId.ToString(),
                RentalId = payment.RentalId.ToString(),
                PaymentMethod = payment.PaymentMethod ?? string.Empty,
                Amount = payment.Amount,
                PaymentTime = payment.PaymentTime,
                Status = payment.Status ?? string.Empty,
                TransactionRef = payment.TransactionRef
            };
        }

        public async Task<List<PaymentResponse>> GetPaymentsByRentalIdAsync(Guid rentalId)
        {
            var payments = await _paymentRepository.GetByRentalIdAsync(rentalId);

            return payments.Select(p => new PaymentResponse
            {
                PaymentId = p.PaymentId.ToString(),
                RentalId = p.RentalId.ToString(),
                PaymentMethod = p.PaymentMethod ?? string.Empty,
                Amount = p.Amount,
                PaymentTime = p.PaymentTime,
                Status = p.Status ?? string.Empty,
                TransactionRef = p.TransactionRef
            }).ToList();
        }

        public async Task<List<PaymentResponse>> GetPaymentsByRenterIdAsync(Guid renterId)
        {
            var payments = await _paymentRepository.GetByRenterIdAsync(renterId);

            return payments.Select(p => new PaymentResponse
            {
                PaymentId = p.PaymentId.ToString(),
                RentalId = p.RentalId.ToString(),
                PaymentMethod = p.PaymentMethod ?? string.Empty,
                Amount = p.Amount,
                PaymentTime = p.PaymentTime,
                Status = p.Status ?? string.Empty,
                TransactionRef = p.TransactionRef
            }).ToList();
        }
    }
}
