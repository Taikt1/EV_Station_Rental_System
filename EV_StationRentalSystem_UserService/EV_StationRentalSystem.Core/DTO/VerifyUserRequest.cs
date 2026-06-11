using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO
{
    public class VerifyUserRequest
    {
        [Required(ErrorMessage = "Status is required")]
        public string Status { get; set; } = null!;
    }
}
