using EV_StationRentalSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IRentalOrderService
    {
        Task<RentalOrderResponse> CreateRentalOrderAsync(CreateRentalOrderRequest request);
        Task<RentalOrderDetailResponse?> GetRentalOrderByIdAsync(Guid rentalId);
        Task<List<RentalOrderResponse>> GetAllRentalOrdersAsync();
        Task<List<RentalOrderResponse>> GetRentalHistoryByRenterIdAsync(Guid renterId, string? status = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<bool> CancelRentalOrderAsync(Guid rentalId, string reason);
        Task<CheckInResponse> CheckInAsync(Guid rentalId, CheckInRequest request);
        Task<CheckOutResponse> CheckOutAsync(Guid rentalId, CheckOutRequest request);
    }
}
