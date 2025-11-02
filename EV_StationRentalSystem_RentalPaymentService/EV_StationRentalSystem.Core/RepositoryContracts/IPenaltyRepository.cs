using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IPenaltyRepository
    {
        Task AddAsync(PenaltyRecord penalty);
        Task<PenaltyRecord?> GetByIdAsync(Guid id);
        Task<IEnumerable<PenaltyRecord>> GetByRentalIdAsync(Guid rentalId);
    }
}
