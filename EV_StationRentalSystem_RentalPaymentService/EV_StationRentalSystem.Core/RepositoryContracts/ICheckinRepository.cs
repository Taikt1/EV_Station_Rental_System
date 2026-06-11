using EV_StationRentalSystem.Core.DTO.Response;
using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface ICheckinRepository
    {
        Task<PagedResponse<Checkin>> GetPagedAsync(int pageIndex, int pageSize);
        Task<Checkin?> GetByOrderIdAsync(Guid orderId);
        Task<Checkin?> GetByIdAsync(Guid id);
        Task<Checkin> AddAsync(Checkin entity);
        Task<Checkin> UpdateAsync(Checkin checkin);
    }
}
