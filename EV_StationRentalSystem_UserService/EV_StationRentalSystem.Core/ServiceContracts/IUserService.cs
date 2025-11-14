using EV_StationRentalSystem.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EV_StationRentalSystem.Core.ServiceContracts
{
    public interface IUserService
    {

        Task<AuthenticationResponse?> Login(LoginRequest loginRequest);
        Task<AuthenticationResponse?> Register(RegisterRequest registerRequest);
        Task<bool> Logout();

        Task<UserProfileResponse?> GetUserProfileAsync(string userId);

        Task<UserProfileResponse?> UpdateProfileAsync(string userId, UpdateProfileRequest request);

        Task<string> UploadDocumentAsync(string userId, UploadDocumentRequest request);

        Task<bool> VerifyUserAsync(string userId, string status);

        // Admin management methods
        Task<UserProfileResponse?> AdminUpdateUserAsync(string userId, AdminUpdateUserRequest request);
        Task<bool> ChangeUserRoleAsync(string userId, string newRole);
        Task<bool> LockUserAsync(string userId, string? reason);
        Task<bool> UnlockUserAsync(string userId);
        Task<bool> DeleteUserAsync(string userId, string? reason);
    }
}
