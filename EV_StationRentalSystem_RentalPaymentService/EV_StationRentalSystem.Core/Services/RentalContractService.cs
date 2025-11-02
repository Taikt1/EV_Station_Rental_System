using AutoMapper;
using EV_StationRentalSystem.Core.DTO.Request;
using EV_StationRentalSystem.Core.DTO.Response;
using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Core.ServiceContracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Services
{
    public class RentalContractService : IRentalContractService
    {
        private readonly IRentalContractRepository _contractRepo;
        private readonly IMapper _mapper;

        public RentalContractService(IRentalContractRepository contractRepo, IMapper mapper)
        {
            _contractRepo = contractRepo;
            _mapper = mapper;
        }

        public async Task<RentalContractResponse> GetByRentalIdAsync(Guid rentalId)
        {
            var contract = await _contractRepo.GetByRentalIdAsync(rentalId)
                ?? throw new KeyNotFoundException("Contract not found for this rental order");

            return _mapper.Map<RentalContractResponse>(contract);
        }

        public async Task<IEnumerable<RentalContractResponse>> GetByUserOrStaffAsync(Guid? renterId, Guid? staffId)
        {
            var list = await _contractRepo.GetByUserOrStaffAsync(renterId, staffId);
            return _mapper.Map<IEnumerable<RentalContractResponse>>(list);
        }

        public async Task<RentalContractResponse> CreateAsync(CreateRentalContractRequest request)
        {
            var entity = _mapper.Map<RentalContract>(request);
            await _contractRepo.AddAsync(entity);
            return _mapper.Map<RentalContractResponse>(entity);
        }

        public async Task<RentalContractResponse> UpdateSignatureAsync(UpdateContractSignatureRequest request)
        {
            var contract = await _contractRepo.GetByIdAsync(request.ContractId)
                ?? throw new KeyNotFoundException("Contract not found");

            if (request.SignedByStaff.HasValue)
                contract.SignedByStaff = request.SignedByStaff.Value;
            if (request.SignedByRenter.HasValue)
                contract.SignedByRenter = request.SignedByRenter.Value;

            await _contractRepo.UpdateAsync(contract);
            return _mapper.Map<RentalContractResponse>(contract);
        }
    }
}
