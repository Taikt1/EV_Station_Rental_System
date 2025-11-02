using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO.Request
{
    public class CreatePenaltyRequest
    {
        public Guid RentalId { get; set; }
        public string? Reason { get; set; }
        public decimal PenaltyAmount { get; set; }
        public Guid IssuedBy { get; set; }  // Staff GUID
    }
}
