using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO.Response
{
    public class PenaltyResponse
    {
        public Guid PenaltyId { get; set; }
        public Guid RentalId { get; set; }
        public string? Reason { get; set; }
        public decimal PenaltyAmount { get; set; }
        public Guid IssuedBy { get; set; }
        public DateTime IssuedDate { get; set; }
    }
}
