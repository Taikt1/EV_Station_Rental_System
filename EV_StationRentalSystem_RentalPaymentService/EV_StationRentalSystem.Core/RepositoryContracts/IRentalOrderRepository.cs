using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface IRentalOrderRepository
    {
        Task<RentalOrder> CreateAsync(RentalOrder rentalOrder);
        Task<RentalOrder?> GetByIdAsync(Guid rentalId);
        Task<List<RentalOrder>> GetAllAsync();
        Task<List<RentalOrder>> GetByRenterIdAsync(Guid renterId);
        Task<RentalOrder> UpdateAsync(RentalOrder rentalOrder);
        Task<bool> DeleteAsync(Guid rentalId);
    }
}
