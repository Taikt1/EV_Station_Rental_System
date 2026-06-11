using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface ICheckoutService
    {
        Task<CheckoutResponse> CreateCheckoutAsync(CreateCheckoutRequest request);
        Task<CheckoutResponse?> GetByIdAsync(Guid id);
        Task<CheckoutResponse?> GetCheckoutByOrderIdAsync(Guid orderDetailId);
        Task<PagedResponse<CheckoutResponse>> GetAllPagedAsync(int pageIndex, int pageSize);
        Task<CheckoutResponse> UpdateStatusAsync(Guid checkoutId, UpdateCheckoutStatusRequest request);
    }
}
