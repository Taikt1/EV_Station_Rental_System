using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class RentalContract
    {
        [Key]
        public int ContractId { get; set; }

        public Guid RentalId { get; set; }  // FK thật trong cùng DB
        public string? ContractType { get; set; }  // Electronic / Paper
        public string? ContractFile { get; set; }
        public int SignedByStaff { get; set; }     // logic ID
        public int SignedByRenter { get; set; }    // logic ID

        public RentalOrder RentalOrder { get; set; }
    }

}
