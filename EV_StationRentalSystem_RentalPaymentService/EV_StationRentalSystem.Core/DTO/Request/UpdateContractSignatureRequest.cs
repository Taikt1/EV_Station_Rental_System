using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO.Request
{
    public class UpdateContractSignatureRequest
    {
        public int ContractId { get; set; }
        public int? SignedByStaff { get; set; }
        public int? SignedByRenter { get; set; }
    }
}
