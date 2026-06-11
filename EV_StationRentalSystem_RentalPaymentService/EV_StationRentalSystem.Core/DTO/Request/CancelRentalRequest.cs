using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO.Request
{
    public class CancelRentalRequest
    {
        [Required(ErrorMessage = "Reason is required")]
        public string Reason { get; set; } = string.Empty;
    }
}

