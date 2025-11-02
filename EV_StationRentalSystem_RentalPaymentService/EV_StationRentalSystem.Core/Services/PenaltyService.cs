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
    public class PenaltyService : IPenaltyService
    {
        private readonly IPenaltyRepository _penaltyRepo;
        private readonly IMapper _mapper;

        public PenaltyService(IPenaltyRepository penaltyRepo, IMapper mapper)
        {
            _penaltyRepo = penaltyRepo;
            _mapper = mapper;
        }

        public async Task<PenaltyResponse> CreatePenaltyAsync(CreatePenaltyRequest request)
        {
            var penalty = new PenaltyRecord
            {
                PenaltyId = Guid.NewGuid(),
                RentalId = request.RentalId,
                Reason = request.Reason,
                PenaltyAmount = request.PenaltyAmount,
                IssuedBy = request.IssuedBy,
                IssuedDate = DateTime.UtcNow
            };

            await _penaltyRepo.AddAsync(penalty);

            return _mapper.Map<PenaltyResponse>(penalty);
        }

        public async Task<PenaltyResponse?> GetPenaltyByIdAsync(Guid id)
        {
            var penalty = await _penaltyRepo.GetByIdAsync(id);
            return penalty == null ? null : _mapper.Map<PenaltyResponse>(penalty);
        }

        public async Task<IEnumerable<PenaltyResponse>> GetPenaltiesByRentalIdAsync(Guid rentalId)
        {
            var penalties = await _penaltyRepo.GetByRentalIdAsync(rentalId);
            return penalties.Select(_mapper.Map<PenaltyResponse>);
        }
    }

}
