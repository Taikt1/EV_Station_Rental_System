using Google.Apis.Auth;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.DTO;

namespace EV_StationRentalSystem.Core.Services
{
    public interface IGoogleAuthService
    {
        Task<GoogleAuthResponse> AuthenticateGoogleUser(string idToken);
    }

    public class GoogleAuthService : IGoogleAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;

        public GoogleAuthService(
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        public async Task<GoogleAuthResponse> AuthenticateGoogleUser(string idToken)
        {
            // Xác thực Google ID Token
            var payload = await ValidateGoogleToken(idToken);

            if (payload == null)
            {
                throw new UnauthorizedAccessException("Google token không hợp lệ");
            }

            // Tìm hoặc tạo user
            var user = await _userManager.FindByEmailAsync(payload.Email);

            if (user == null)
            {
                // Tạo user mới từ Google account
                user = new ApplicationUser
                {
                    UserName = payload.Email,
                    Email = payload.Email,
                    EmailConfirmed = true, // Google đã xác thực email
                    Status = "Active"
                };

                var result = await _userManager.CreateAsync(user);

                if (!result.Succeeded)
                {
                    throw new Exception("Không thể tạo user từ tài khoản Google");
                }

                // Gán role mặc định là customer
                await _userManager.AddToRoleAsync(user, "customer");
            }

            // Lấy roles của user
            var roles = await _userManager.GetRolesAsync(user);

            // Tạo JWT token
            var token = GenerateJwtToken(user, roles.ToList());

            return new GoogleAuthResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                FullName = user.UserProfile?.FullName ?? payload.Name ?? user.Email!,
                Token = token,
                Roles = roles.ToList()
            };
        }

        private async Task<GoogleJsonWebSignature.Payload?> ValidateGoogleToken(string idToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings
                {
                    Audience = new[] { _configuration["GoogleAuth:ClientId"]! }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(idToken, settings);
                return payload;
            }
            catch (Exception)
            {
                return null;
            }
        }

        private string GenerateJwtToken(ApplicationUser user, List<string> roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName!),
                new Claim(ClaimTypes.Email, user.Email!)
            };

            // Thêm roles vào claims
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JwtSettings:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["JwtSettings:Issuer"],
                audience: _configuration["JwtSettings:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(Convert.ToDouble(_configuration["JwtSettings:ExpiryInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
