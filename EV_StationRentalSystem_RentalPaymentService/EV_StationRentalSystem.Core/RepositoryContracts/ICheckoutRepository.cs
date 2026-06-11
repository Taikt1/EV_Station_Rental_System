using EV_StationRentalSystem.Core.DTO.Response;
using EV_StationRentalSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.RepositoryContracts
{
    public interface ICheckoutRepository
    {
        Task<PagedResponse<Checkout>> GetPagedAsync(int pageIndex, int pageSize);
        Task<Checkout?> GetByOrderDetailIdAsync(Guid orderDetailId);
        Task<Checkout?> GetByIdAsync(Guid id);
        Task<Checkout> AddAsync(Checkout checkout);
        Task<Checkout> UpdateAsync(Checkout checkout);
    }
}
