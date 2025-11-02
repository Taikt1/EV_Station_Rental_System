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
        private readonly IMapper _mapper;

        public CheckoutService(ICheckoutRepository checkoutRepo, IMapper mapper)
        {
            _checkoutRepo = checkoutRepo;
            _mapper = mapper;
        }

        public async Task<CheckoutResponse> CreateCheckoutAsync(CreateCheckoutRequest request)
        {
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

            // Ảnh chứng minh
            if (request.Photos != null)
            {
                checkout.PhotoProofs = request.Photos.Select(url => new PhotoProof
                {
                    PhotoId = Guid.NewGuid(),
                    CheckoutId = checkout.CheckoutId,
                    PhotoUrl = url.PhotoUrl,
                    CapturedAt = DateTime.UtcNow,
                    Description = "Checkout Proof"
                }).ToList();
            }

            await _checkoutRepo.AddAsync(checkout);

            return _mapper.Map<CheckoutResponse>(checkout);
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
