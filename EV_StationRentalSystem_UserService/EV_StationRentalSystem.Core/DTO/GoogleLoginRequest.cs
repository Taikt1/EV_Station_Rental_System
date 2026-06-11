namespace EV_StationRentalSystem.Core.DTO
{
    public class GoogleLoginRequest
    {
        public string IdToken { get; set; } = string.Empty;
    }

    public class GoogleAuthResponse
    {
        public string UserId { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new List<string>();
    }
}
