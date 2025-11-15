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
        private readonly IRentalContractRepository _contractRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IMapper _mapper;

        public CheckinService(
            ICheckinRepository repo, 
            IMapper mapper, 
            IRentalOrderRepository orderRepository,
            IRentalContractRepository contractRepository,
            IPaymentRepository paymentRepository)
        {
            _repo = repo;
            _mapper = mapper;
            _orderRepository = orderRepository;
            _contractRepository = contractRepository;
            _paymentRepository = paymentRepository;
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
            // Validate that the RentalOrderDetailId exists in the database
            var rentalOrderDetail = await _orderRepository.GetOrderDetailByIdAsync(request.RentalOrderDetailId);
            
            if (rentalOrderDetail == null)
            {
                // Get all rental order details to help debug
                var allOrders = await _orderRepository.GetAllAsync();
                var availableDetails = allOrders
                    .Where(ro => ro.RentalOrderDetails != null)
                    .SelectMany(ro => ro.RentalOrderDetails.Select(d => new { 
                        RentalOrderId = ro.RentalId, 
                        DetailId = d.Id,
                        VehicleId = d.VehicleId,
                        Status = ro.Status
                    }))
                    .Take(10)
                    .ToList();
                
                var debugInfo = availableDetails.Any() 
                    ? $"Available RentalOrderDetailIds: {string.Join(", ", availableDetails.Select(d => $"{d.DetailId} (RentalOrder: {d.RentalOrderId}, Vehicle: {d.VehicleId}, Status: {d.Status})"))}"
                    : "No RentalOrderDetails found in database. Please create a RentalOrder first.";
                
                throw new Exception($"RentalOrderDetail with ID {request.RentalOrderDetailId} not found. {debugInfo}");
            }

            // ✅ GET RENTAL ORDER
            var rentalOrder = await _orderRepository.GetRentalOrderByDetailIdAsync(request.RentalOrderDetailId);
            if (rentalOrder == null)
                throw new Exception("Không tìm thấy đơn thuê xe");

            // ✅ VALIDATE 1: KIỂM TRA HỢP ĐỒNG ĐÃ KÝ
            var contract = await _contractRepository.GetByRentalIdAsync(rentalOrder.RentalId);
            if (contract == null)
                throw new Exception("Chưa có hợp đồng cho đơn thuê xe này");
            
            if (contract.SignedByRenter == 0 || contract.SignedByStaff == 0)
                throw new Exception($"Hợp đồng chưa được ký đầy đủ. Staff: {(contract.SignedByStaff == 1 ? "Đã ký" : "Chưa ký")}, Renter: {(contract.SignedByRenter == 1 ? "Đã ký" : "Chưa ký")}");

            // ✅ VALIDATE 2: KIỂM TRA ĐÃ THANH TOÁN ĐẶT CỌC
            var payments = await _paymentRepository.GetByRentalIdAsync(rentalOrder.RentalId);
            var depositPayment = payments.FirstOrDefault(p => 
                p.TransactionRef != null && 
                p.TransactionRef.Contains("DEPOSIT") && 
                p.Status == "Paid");
            
            if (depositPayment == null)
                throw new Exception("Chưa thanh toán tiền đặt cọc. Vui lòng thanh toán trước khi nhận xe");
            
            // ✅ VALIDATE 3: KIỂM TRA STATUS
            if (rentalOrder.Status != "Pending" && rentalOrder.Status != "Confirmed")
                throw new Exception($"Không thể check-in với trạng thái hiện tại: {rentalOrder.Status}");
            

            // ✅ TẠO CHECKIN RECORD
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
                    RentalId = rentalOrder.RentalId,  
                    RentalOrderRentalId = rentalOrder.RentalId, 
                    PhotoUrl = url.PhotoUrl,
                    CapturedAt = DateTime.UtcNow,
                    Description = url.Description ?? "Check-in photo"
                }).ToList() ?? new List<PhotoProof>()
            };

            // ✅ CẬP NHẬT TRẠNG THÁI ĐƠN THUÊ → ACTIVE
            rentalOrder.Status = "Active";
            await _orderRepository.UpdateAsync(rentalOrder);

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
