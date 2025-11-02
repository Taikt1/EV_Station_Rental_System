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
    public class CheckinService : ICheckinService
    {
        private readonly ICheckinRepository _repo;
        private readonly IRentalOrderRepository _orderRepository;
        private readonly IMapper _mapper;

        public CheckinService(ICheckinRepository repo, IMapper mapper, IRentalOrderRepository orderRepository)
        {
            _repo = repo;
            _mapper = mapper;
            _orderRepository = orderRepository;
        }

        public async Task<PagedResponse<CheckinResponse>> GetPagedAsync(int pageIndex, int pageSize)
        {
            var pagedData = await _repo.GetPagedAsync(pageIndex, pageSize);
            var mappedItems = _mapper.Map<List<CheckinResponse>>(pagedData.Data);

            return new PagedResponse<CheckinResponse>
            {
                TotalCount = pagedData.TotalCount,
                PageIndex = pagedData.PageIndex,
                PageSize = pagedData.PageSize,
                Data = mappedItems
            };
        }

        public async Task<CheckinResponse?> GetByOrderIdAsync(Guid orderId)
        {
            var checkin = await _repo.GetByOrderIdAsync(orderId);
            return checkin == null ? null : _mapper.Map<CheckinResponse>(checkin);
        }

        public async Task<CheckinResponse?> GetByIdAsync(Guid id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity == null ? null : _mapper.Map<CheckinResponse>(entity);
        }

        public async Task<CheckinResponse> CreateAsync(CreateCheckinRequest request)
        {
            // lấy rentalId thật từ RentalOrderDetail
            var rentalOrderDetail = await _orderRepository.GetOrderDetailsAsync(request.RentalOrderDetailId);

            if (rentalOrderDetail == null)
                throw new Exception("RentalOrderDetail not found");

            var checkin = new Checkin
            {
                CheckinId = Guid.NewGuid(),
                RentalOrderDetailId = request.RentalOrderDetailId,
                StaffId = request.StaffId,
                Datetime = DateTime.UtcNow,
                OdometerReading = request.OdometerReading,
                BatteryLevel = request.BatteryLevel,
                Status = request.Status,
                PhotoProofs = request.Photos?.Select(url => new PhotoProof
                {
                    PhotoId = Guid.NewGuid(),
                    PhotoUrl = url.PhotoUrl,
                    CapturedAt = DateTime.UtcNow,
                    Description = "Check-in photo"
                }).ToList() ?? new List<PhotoProof>()
            };
            var response = await _repo.AddAsync(checkin);
            return _mapper.Map<CheckinResponse>(response);
        }

        public async Task<CheckinResponse> UpdateStatusAsync(Guid checkinId, UpdateCheckinStatusRequest request)
        {
            var checkin = await _repo.GetByIdAsync(checkinId);
            if (checkin == null)
                return null;

            checkin.Status = request.Status;
            var updated = await _repo.UpdateAsync(checkin);
            return _mapper.Map<CheckinResponse>(updated);
        }
    }
}
