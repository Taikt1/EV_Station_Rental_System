using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.DTO
{
    public record AuthenticationResponse(
        Guid UserId,
        string? UserName,
        string? Email,
        string? Phone,
        string? Token,
        bool Success
     )
    {
        public AuthenticationResponse() : this(Guid.Empty, null, null, null, null, false) { } 
    }
    
}
