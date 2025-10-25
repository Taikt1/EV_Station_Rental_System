using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO
{
    public record RegisterRequest(
        string? FirstName,
        string? LastName,
        string? Email,
        string? Password,
        string? Phone
    );
}
