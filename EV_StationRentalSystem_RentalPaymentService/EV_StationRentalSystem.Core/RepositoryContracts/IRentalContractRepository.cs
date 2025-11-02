using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IRentalContractRepository
    {
        Task<RentalContract?> GetByIdAsync(int contractId);
        Task<RentalContract?> GetByRentalIdAsync(Guid rentalId);
        Task<IEnumerable<RentalContract>> GetByUserOrStaffAsync(Guid? renterId, Guid? staffId);
        Task AddAsync(RentalContract contract);
        Task UpdateAsync(RentalContract contract);
    }
}
