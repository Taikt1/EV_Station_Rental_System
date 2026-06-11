using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO.Request
{
    public class CreateRentalContractRequest
    {
        public Guid RentalId { get; set; }
        public string? ContractType { get; set; }
        public string? ContractFile { get; set; }
    }
}
