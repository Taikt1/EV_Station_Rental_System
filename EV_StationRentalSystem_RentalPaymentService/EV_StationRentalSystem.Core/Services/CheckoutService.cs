using AutoMapper;
using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.DTO.Response;
using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly ICheckoutRepository _checkoutRepo;
        private readonly IRentalOrderRepository _orderRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IPenaltyRepository _penaltyRepository;
        private readonly IMapper _mapper;

        public CheckoutService(
            ICheckoutRepository checkoutRepo, 
            IMapper mapper, 
            IRentalOrderRepository orderRepository,
            IPaymentRepository paymentRepository,
            IPenaltyRepository penaltyRepository)
        {
            _checkoutRepo = checkoutRepo;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _paymentRepository = paymentRepository;
            _penaltyRepository = penaltyRepository;
        }

        public async Task<CheckoutResponse> CreateCheckoutAsync(CreateCheckoutRequest request)
        {
            var rentalOrderDetail = await _orderRepository.GetOrderDetailByIdAsync(request.RentalOrderDetailId);
            
            if (rentalOrderDetail == null)
            {
                throw new Exception($"RentalOrderDetail with ID {request.RentalOrderDetailId} not found");
            }

            var rentalOrder = await _orderRepository.GetRentalOrderByDetailIdAsync(request.RentalOrderDetailId);
            if (rentalOrder == null)
            {
                throw new Exception($"RentalOrder not found for RentalOrderDetail {request.RentalOrderDetailId}");
            }

            var checkout = new Checkout
            {
                CheckoutId = Guid.NewGuid(),
                RentalOrderDetailId = request.RentalOrderDetailId,
                StaffId = request.StaffId,
                Datetime = DateTime.UtcNow,
                OdometerReading = request.OdometerReading,
                BatteryLevel = request.BatteryLevel,
                ExtraFee = request.ExtraFee,
                Status = request.Status
            };

            // Calculate extra fees
            if (request.BatteryLevel < 50)
            {
                var fee = (50 - request.BatteryLevel) * 10000; // 10k per % under 50%
                checkout.ExtraFee += fee;
            }

            // Ảnh chứng minh - SET CẢ 2 CỘT RentalId
            if (request.Photos != null)
            {
                checkout.PhotoProofs = request.Photos.Select(url => new PhotoProof
                {
                    PhotoId = Guid.NewGuid(),
                    CheckoutId = checkout.CheckoutId,
                    RentalId = rentalOrder.RentalId,  // ✅ Set cột RentalId (cũ)
                    RentalOrderRentalId = rentalOrder.RentalId,  // ✅ Set cột RentalOrderRentalId (shadow)
                    PhotoUrl = url.PhotoUrl,
                    CapturedAt = DateTime.UtcNow,
                    Description = "Checkout Proof"
                }).ToList();
            }

            await _checkoutRepo.AddAsync(checkout);

            await CreateFinalPaymentAsync(request.RentalOrderDetailId, checkout.ExtraFee ?? 0);

            return _mapper.Map<CheckoutResponse>(checkout);
        }

        private async Task CreateFinalPaymentAsync(Guid rentalOrderDetailId, decimal extraFee)
        {
            var rentalOrder = await _orderRepository.GetRentalOrderByDetailIdAsync(rentalOrderDetailId);
            if (rentalOrder == null) return;

            var payments = await _paymentRepository.GetByRentalIdAsync(rentalOrder.RentalId);
            
            var depositAmount = payments
                .Where(p => p.TransactionRef != null && p.TransactionRef.Contains("DEPOSIT") && p.Status == "Paid")
                .Sum(p => p.Amount);

            var penalties = await _penaltyRepository.GetByRentalIdAsync(rentalOrder.RentalId);
            var penaltyAmount = penalties.Sum(p => p.PenaltyAmount);

            // Calculate final amount
            var estimatedCost = rentalOrder.EstimatedCost;
            var actualCost = estimatedCost + extraFee + penaltyAmount;
            var finalAmount = actualCost - depositAmount;

            rentalOrder.ActualCost = actualCost;
            rentalOrder.Status = "Completed";
            await _orderRepository.UpdateAsync(rentalOrder);

            // Luôn tạo payment record, kể cả khi amount = 0
            var payment = new Payment
            {
                PaymentId = Guid.NewGuid(),
                RentalId = rentalOrder.RentalId,
                Amount = Math.Abs(finalAmount),
                PaymentMethod = finalAmount == 0 ? "None" : "Pending", // None nếu không cần thanh toán
                PaymentTime = DateTime.UtcNow,
                Status = finalAmount == 0 ? "Paid" : "Pending", // Paid nếu amount = 0
                TransactionRef = finalAmount > 0 ? "FINAL_PAYMENT" 
                               : finalAmount < 0 ? "REFUND" 
                               : "NO_ADDITIONAL_PAYMENT" // Amount = 0
            };

            await _paymentRepository.CreateAsync(payment);
        }

        public async Task<CheckoutResponse?> GetCheckoutByOrderIdAsync(Guid orderDetailId)
        {
            var checkout = await _checkoutRepo.GetByOrderDetailIdAsync(orderDetailId);
            return checkout == null ? null : _mapper.Map<CheckoutResponse>(checkout);
        }

        public async Task<CheckoutResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _checkoutRepo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<CheckoutResponse>(entity);
        }

        public async Task<PagedResponse<CheckoutResponse>> GetAllPagedAsync(int pageIndex, int pageSize)
        {
            var paged = await _checkoutRepo.GetPagedAsync(pageIndex, pageSize);
            var mappedItems = _mapper.Map<List<CheckoutResponse>>(paged.Data);
            return new PagedResponse<CheckoutResponse>
            {
                TotalCount = paged.TotalCount,
                PageIndex = paged.PageIndex,
                PageSize = paged.PageSize,
                Data = mappedItems
            };
        }

        public async Task<CheckoutResponse> UpdateStatusAsync(Guid checkoutId, UpdateCheckoutStatusRequest request)
        {
            var checkout = await _checkoutRepo.GetByIdAsync(checkoutId);
            if (checkout == null)
                return null;

            checkout.Status = request.Status;
            var updated = await _checkoutRepo.UpdateAsync(checkout);
            return _mapper.Map<CheckoutResponse>(updated);
        }
    }
}
