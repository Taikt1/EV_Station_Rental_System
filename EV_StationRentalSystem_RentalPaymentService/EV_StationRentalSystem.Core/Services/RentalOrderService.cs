using AutoMapper;
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
    public class RentalOrderService : IRentalOrderService
    {
        private readonly IRentalOrderRepository _rentalOrderRepository;
        private readonly IMapper _mapper;

        public RentalOrderService(IRentalOrderRepository rentalOrderRepository, IMapper mapper)
        {
            _rentalOrderRepository = rentalOrderRepository;
            _mapper = mapper;
        }

        public async Task<RentalOrderResponse> CreateRentalOrderAsync(CreateRentalOrderRequest request)
        {
            // Validate and parse GUIDs
            if (!Guid.TryParse(request.RenterId, out var renterId))
                throw new ArgumentException("Invalid RenterId format");
            if (!Guid.TryParse(request.BranchStartId, out var branchStartId))
                throw new ArgumentException("Invalid BranchStartId format");
            if (!Guid.TryParse(request.BranchEndId, out var branchEndId))
                throw new ArgumentException("Invalid BranchEndId format");

            // Create new rental order entity
            var rentalOrder = new RentalOrder
            {
                RentalId = Guid.NewGuid(),
                RenterId = renterId,
                StaffId = request.StaffId,
                VehicleId = request.Details.Select(d => d.VehicleId).First(),
                BranchStartId = branchStartId,
                BranchEndId = branchEndId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                Status = "Pending",
                EstimatedCost = request.EstimatedCost,
                ActualCost = null,
                RentalOrderDetails = request.Details.Select(d => new RentalOrderDetail
                {
                    Id = Guid.NewGuid(),
                    VehicleId = d.VehicleId,
                    AssignedAt = DateTime.UtcNow
                }).ToList()
            };

            var createdOrder = await _rentalOrderRepository.CreateAsync(rentalOrder);

            return new RentalOrderResponse
            {
                RentalId = createdOrder.RentalId.ToString(),
                RenterId = createdOrder.RenterId.ToString(),
                StaffId = createdOrder.StaffId?.ToString(),
                VehicleId = createdOrder.VehicleId.ToString(),
                BranchStartId = createdOrder.BranchStartId.ToString(),
                BranchEndId = createdOrder.BranchEndId.ToString(),
                StartTime = createdOrder.StartTime,
                EndTime = createdOrder.EndTime,
                Status = createdOrder.Status,
                EstimatedCost = createdOrder.EstimatedCost,
                ActualCost = createdOrder.ActualCost,
                CreatedAt = DateTime.UtcNow
            };
        }

        public async Task<List<RentalOrderResponse>> GetAllRentalOrdersAsync()
        {
            var rentalOrders = await _rentalOrderRepository.GetAllAsync();

            return rentalOrders.Select(ro => new RentalOrderResponse
            {
                RentalId = ro.RentalId.ToString(),
                RenterId = ro.RenterId.ToString(),
                StaffId = ro.StaffId?.ToString(),
                VehicleId = ro.VehicleId.ToString(),
                BranchStartId = ro.BranchStartId.ToString(),
                BranchEndId = ro.BranchEndId.ToString(),
                StartTime = ro.StartTime,
                EndTime = ro.EndTime,
                Status = ro.Status,
                EstimatedCost = ro.EstimatedCost,
                ActualCost = ro.ActualCost,
                CreatedAt = DateTime.UtcNow
            }).ToList();
        }

        public async Task<RentalOrderDetailResponse?> GetRentalOrderByIdAsync(Guid rentalId)
        {
            var rentalOrder = await _rentalOrderRepository.GetByIdAsync(rentalId);
            
            if (rentalOrder == null)
                return null;

            return new RentalOrderDetailResponse
            {
                RentalId = rentalOrder.RentalId.ToString(),
                RenterId = rentalOrder.RenterId.ToString(),
                StaffId = rentalOrder.StaffId?.ToString(),
                VehicleId = rentalOrder.VehicleId.ToString(),
                BranchStartId = rentalOrder.BranchStartId.ToString(),
                BranchEndId = rentalOrder.BranchEndId.ToString(),
                StartTime = rentalOrder.StartTime,
                EndTime = rentalOrder.EndTime,
                Status = rentalOrder.Status,
                EstimatedCost = rentalOrder.EstimatedCost,
                ActualCost = rentalOrder.ActualCost,
                Payments = rentalOrder.Payments?.Select(p => new PaymentInfo
                {
                    PaymentId = p.PaymentId.ToString(),
                    Amount = p.Amount,
                    PaymentMethod = p.PaymentMethod ?? string.Empty,
                    Status = p.Status ?? string.Empty,
                    PaymentDate = p.PaymentTime
                }).ToList(),
                Feedbacks = rentalOrder.FeedbackRatings?.Select(f => new FeedbackInfo
                {
                    FeedbackId = f.FeedbackId.ToString(),
                    Rating = f.Score,
                    Comment = f.Comment,
                    CreatedAt = f.CreatedAt
                }).ToList()
            };
        }

        public async Task<List<RentalOrderResponse>> GetRentalHistoryByRenterIdAsync(Guid renterId, string? status = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var rentalOrders = await _rentalOrderRepository.GetByRenterIdAsync(renterId);

            // Apply filters
            if (!string.IsNullOrEmpty(status))
            {
                rentalOrders = rentalOrders.Where(o => o.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
            }

            if (fromDate.HasValue)
            {
                rentalOrders = rentalOrders.Where(o => o.StartTime >= fromDate.Value).ToList();
            }

            if (toDate.HasValue)
            {
                rentalOrders = rentalOrders.Where(o => o.StartTime <= toDate.Value).ToList();
            }

            return rentalOrders.Select(order => new RentalOrderResponse
            {
                RentalId = order.RentalId.ToString(),
                RenterId = order.RenterId.ToString(),
                StaffId = order.StaffId?.ToString(),
                VehicleId = order.VehicleId.ToString(),
                BranchStartId = order.BranchStartId.ToString(),
                BranchEndId = order.BranchEndId.ToString(),
                StartTime = order.StartTime,
                EndTime = order.EndTime,
                Status = order.Status,
                EstimatedCost = order.EstimatedCost,
                ActualCost = order.ActualCost,
                CreatedAt = order.StartTime
            }).ToList();
        }

        public async Task<bool> CancelRentalOrderAsync(Guid rentalId, string reason)
        {
            var rentalOrder = await _rentalOrderRepository.GetByIdAsync(rentalId);
            
            if (rentalOrder == null)
            {
                return false;
            }

            if (rentalOrder.Status == "Cancelled")
            {
                throw new InvalidOperationException("Đơn thuê này đã bị hủy trước đó");
            }
            
            if (rentalOrder.Status == "Completed")
            {
                throw new InvalidOperationException("Không thể hủy đơn thuê đã hoàn thành");
            }
            
            if (rentalOrder.Status == "Active")
            {
                throw new InvalidOperationException("Không thể hủy đơn thuê đang thực hiện. Vui lòng trả xe trước");
            }

            rentalOrder.Status = "Cancelled";
            await _rentalOrderRepository.UpdateAsync(rentalOrder);
            
            return true;
        }

        public async Task<RentalOrderResponse> UpdateStatusAsync(Guid id, string status)
        {
            var updated = await _rentalOrderRepository.UpdateStatusAsync(id, status);
            return _mapper.Map<RentalOrderResponse>(updated);
        }

        public async Task<IEnumerable<RentalOrderDetailInfoResponse>> GetOrderDetailsAsync(Guid orderId)
        {
            var details = await _rentalOrderRepository.GetOrderDetailsAsync(orderId);
            return _mapper.Map<IEnumerable<RentalOrderDetailInfoResponse>>(details);
        }

        //public async Task<CheckInResponse> CheckInAsync(Guid rentalId, CheckInRequest request)
        //{
        //    var rentalOrder = await _rentalOrderRepository.GetByIdAsync(rentalId);

        //    if (rentalOrder == null)
        //    {
        //        throw new Exception("Rental order not found");
        //    }

        //    // Validate and parse StaffId
        //    if (!Guid.TryParse(request.StaffId, out var staffId))
        //        throw new ArgumentException("Invalid StaffId format");

        //    // Create checkin record
        //    var checkin = new Checkin
        //    {
        //        CheckinId = Guid.NewGuid(),
        //        RentalOrderDetailId = rentalOrder.RentalOrderDetails?.FirstOrDefault()?.Id ?? Guid.NewGuid(),
        //        StaffId = staffId,
        //        Datetime = DateTime.UtcNow,
        //        OdometerReading = request.OdometerReading,
        //        BatteryLevel = request.BatteryLevel,
        //        Status = "Confirmed"
        //    };

        //    // Update rental order status
        //    rentalOrder.Status = "Active";
        //    await _rentalOrderRepository.UpdateAsync(rentalOrder);

        //    return new CheckInResponse
        //    {
        //        CheckinId = checkin.CheckinId.ToString(),
        //        RentalId = rentalId.ToString(),
        //        Datetime = checkin.Datetime,
        //        OdometerReading = checkin.OdometerReading,
        //        BatteryLevel = checkin.BatteryLevel,
        //        Status = checkin.Status,
        //        ContractUrl = $"/contracts/{rentalId}",
        //        Message = "Check-in successful"
        //    };
        //}

        //public async Task<CheckOutResponse> CheckOutAsync(Guid rentalId, CheckOutRequest request)
        //{
        //    var rentalOrder = await _rentalOrderRepository.GetByIdAsync(rentalId);

        //    if (rentalOrder == null)
        //    {
        //        throw new Exception("Rental order not found");
        //    }

        //    // Validate and parse StaffId
        //    if (!Guid.TryParse(request.StaffId, out var staffId))
        //        throw new ArgumentException("Invalid StaffId format");

        //    // Create checkout record
        //    var checkout = new Checkout
        //    {
        //        CheckoutId = Guid.NewGuid(),
        //        RentalOrderDetailId = rentalOrder.RentalOrderDetails?.FirstOrDefault()?.Id ?? Guid.NewGuid(),
        //        StaffId = staffId,
        //        Datetime = DateTime.UtcNow,
        //        OdometerReading = request.OdometerReading,
        //        BatteryLevel = request.BatteryLevel,
        //        ExtraFee = 0,
        //        Status = "Completed"
        //    };

        //    // Calculate extra fees
        //    var additionalFees = new List<AdditionalFeeInfo>();

        //    if (request.BatteryLevel < 50)
        //    {
        //        var fee = (50 - request.BatteryLevel) * 10000; // 10k per % under 50%
        //        checkout.ExtraFee += fee;
        //        additionalFees.Add(new AdditionalFeeInfo
        //        {
        //            Type = "low_battery",
        //            Description = $"Pin dưới 50% ({request.BatteryLevel}%)",
        //            Amount = fee
        //        });
        //    }

        //    // Update rental order
        //    rentalOrder.Status = "Completed";
        //    rentalOrder.EndTime = DateTime.UtcNow;
        //    rentalOrder.ActualCost = rentalOrder.EstimatedCost + checkout.ExtraFee;
        //    await _rentalOrderRepository.UpdateAsync(rentalOrder);

        //    return new CheckOutResponse
        //    {
        //        CheckoutId = checkout.CheckoutId.ToString(),
        //        RentalId = rentalId.ToString(),
        //        Datetime = checkout.Datetime,
        //        OdometerReading = checkout.OdometerReading,
        //        BatteryLevel = checkout.BatteryLevel,
        //        ExtraFee = checkout.ExtraFee ?? 0,
        //        TotalCost = rentalOrder.ActualCost ?? 0,
        //        Deposit = 1000000, // Fixed deposit for now
        //        RefundAmount = 1000000 - (rentalOrder.ActualCost ?? 0),
        //        AdditionalFees = additionalFees,
        //        Status = checkout.Status ?? "Completed",
        //        Message = "Check-out successful"
        //    };
        //}
    }
}
