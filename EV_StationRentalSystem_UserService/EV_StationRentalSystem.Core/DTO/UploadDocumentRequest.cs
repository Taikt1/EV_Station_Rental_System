using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace EV_StationRentalSystem.Core.DTO
{
    public class UploadDocumentRequest
    {
        [Required]
        public string DocumentType { get; set; } = string.Empty; // "CCCD", "GPLX", "Avatar"

        [Required]
        public IFormFile File { get; set; } = null!;
    }
}
