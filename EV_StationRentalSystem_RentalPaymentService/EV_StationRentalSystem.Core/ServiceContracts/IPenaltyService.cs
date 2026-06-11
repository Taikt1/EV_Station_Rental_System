using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IPenaltyService
    {
        Task<PenaltyResponse> CreatePenaltyAsync(CreatePenaltyRequest request);
        Task<PenaltyResponse?> GetPenaltyByIdAsync(Guid id);
        Task<IEnumerable<PenaltyResponse>> GetPenaltiesByRentalIdAsync(Guid rentalId);
    }
}
