using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IShiftRepository
    {
        Task<Shift?> GetByIdAsync(Guid shiftId);
        Task<List<Shift>> GetAllAsync();
        Task<Shift> AddAsync(Shift shift);
        Task<Shift> UpdateAsync(Shift shift);
        Task<bool> DeleteAsync(Guid shiftId);
        Task<bool> ExistsAsync(Guid shiftId);
    }
}
