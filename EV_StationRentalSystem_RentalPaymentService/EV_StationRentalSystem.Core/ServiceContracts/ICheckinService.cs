using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface ICheckinService
    {
        Task<PagedResponse<CheckinResponse>> GetPagedAsync(int pageIndex, int pageSize);
        Task<CheckinResponse?> GetByOrderIdAsync(Guid orderId);
        Task<CheckinResponse?> GetByIdAsync(Guid id);
        Task<CheckinResponse> CreateAsync(CreateCheckinRequest request);
        Task<CheckinResponse> UpdateStatusAsync(Guid checkinId, UpdateCheckinStatusRequest request);
    }
}
