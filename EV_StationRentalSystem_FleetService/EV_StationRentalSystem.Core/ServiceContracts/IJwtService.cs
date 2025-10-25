using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IJwtService
    {
        ClaimsPrincipal? ValidateToken(string token);
        string? GetUserIdFromToken(string token);
    }
}
