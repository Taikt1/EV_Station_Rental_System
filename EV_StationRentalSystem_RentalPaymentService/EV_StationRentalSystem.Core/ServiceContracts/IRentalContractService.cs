using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.DTO.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IRentalContractService
    {
        Task<RentalContractResponse> GetByRentalIdAsync(Guid rentalId);
        Task<IEnumerable<RentalContractResponse>> GetByUserOrStaffAsync(Guid? renterId, Guid? staffId);
        Task<RentalContractResponse> CreateAsync(CreateRentalContractRequest request);
        Task<RentalContractResponse> UpdateSignatureAsync(UpdateContractSignatureRequest request);
    }
}
