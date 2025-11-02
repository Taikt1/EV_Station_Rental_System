using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO.Response
{
    public class RentalContractResponse
    {
        public int ContractId { get; set; }
        public Guid RentalId { get; set; }
        public string? ContractType { get; set; }
        public string? ContractFile { get; set; }
        public int SignedByStaff { get; set; }
        public int SignedByRenter { get; set; }
    }
}
