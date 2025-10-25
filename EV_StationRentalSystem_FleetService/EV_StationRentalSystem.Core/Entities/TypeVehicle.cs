using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.Entities
{
    public class TypeVehicle
    {
        [Key]
        public Guid TypeId { get; set; }

        public string? TypeName { get; set; } // EV Bike, EV Car, ...
        public string? Brand { get; set; }
        public string? Model { get; set; }
        public double DefaultBattery { get; set; }
        public decimal BasePrice { get; set; }
        public string? Description { get; set; }

        public ICollection<Vehicle> Vehicles { get; set; } = null!;
    }

}
