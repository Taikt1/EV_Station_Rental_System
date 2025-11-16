

using AutoMapper;
using EV_StationRentalSystem.Core.DTO;
using EV_StationRentalSystem.Core.Entities;
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;

namespace EV_StationRentalSystem.Core.Services
{
    internal class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private readonly IJwtService _jwtService;
        private readonly IUserProfileRepository _userProfileRepository;

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper, IJwtService jwtService, IUserProfileRepository userProfileRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _jwtService = jwtService;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<AuthenticationResponse?> Login(LoginRequest loginRequest)
        {
            // Find user by email
            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return new AuthenticationResponse(Guid.Empty, null, null, null, null, false);
            }

            // Check password
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);
            if (!result.Succeeded)
            {
                return new AuthenticationResponse(Guid.Empty, null, null, null, null, false);
            }

            // Generate JWT token
            string token = _jwtService.GenerateToken(user);

            // Map to response with JWT token
            return _mapper.Map<AuthenticationResponse>(user) with { Token = token, Success = true };
        }

        public async Task<AuthenticationResponse?> Register(RegisterRequest registerRequest)
        {
            // Create new user
            var newUser = new ApplicationUser
            {
                UserName = registerRequest.Email, // Identity uses Email as UserName
                Email = registerRequest.Email,
                PhoneNumber = registerRequest.Phone.ToString(),
                Status = "Active"
            };

            // Create user with password
            var result = await _userManager.CreateAsync(newUser, registerRequest.Password);

            if (!result.Succeeded)
            {
                return new AuthenticationResponse(Guid.Empty, null, null, null, null, false);
            }

            // Assign default role (customer)
            await _userManager.AddToRoleAsync(newUser, "customer");

            // Generate JWT token
            string token = _jwtService.GenerateToken(newUser);

            return _mapper.Map<AuthenticationResponse>(newUser) with
            {
                Token = token,
                Success = true
            };
        }

        public async Task<bool> Logout()
        {
            // For JWT-based authentication, logout is typically handled on the client side
            // by removing the token from local storage/cookies.
            // However, we can sign out from ASP.NET Identity's sign-in manager
            // to clear any server-side session/cookies if they exist.
            await _signInManager.SignOutAsync();
            return true;
        }

        public async Task<UserProfileResponse?> GetUserProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            var role = roles.FirstOrDefault() ?? "customer";

            return new UserProfileResponse
            {
                UserId = user.Id,
                Email = user.Email ?? string.Empty,
                UserName = user.UserName ?? string.Empty,
                FullName = userProfile?.FullName ?? string.Empty,
                Dob = userProfile?.Dob,
                Address = userProfile?.Address ?? string.Empty,
                AvatarUrl = userProfile?.AvatarUrl ?? string.Empty,
                CCCDUrl = userProfile?.CCCDUrl ?? string.Empty,
                PhoneNumber = user.PhoneNumber ?? string.Empty,
                Status = user.Status ?? "Active",
                Role = role
            };
        }

        public async Task<UserProfileResponse?> UpdateProfileAsync(string userId, UpdateProfileRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            // Update phone number on ApplicationUser
            user.PhoneNumber = request.PhoneNumber;
            var userResult = await _userManager.UpdateAsync(user);
            if (!userResult.Succeeded)
            {
                return null;
            }

            // Update or create UserProfile
            var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (userProfile == null)
            {
                // Create new profile
                userProfile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FullName = request.FullName,
                    Dob = request.Dob,
                    Address = request.Address ?? string.Empty,
                    AvatarUrl = string.Empty,
                    CCCDUrl = string.Empty
                };
                await _userProfileRepository.AddAsync(userProfile);
            }
            else
            {
                // Update existing profile
                userProfile.FullName = request.FullName;
                userProfile.Dob = request.Dob;
                userProfile.Address = request.Address ?? string.Empty;
                await _userProfileRepository.UpdateAsync(userProfile);
            }

            return await GetUserProfileAsync(userId);
        }

        public async Task<string> UploadDocumentAsync(string userId, UploadDocumentRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new Exception("User not found");
            }

            // Get or create user profile
            var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (userProfile == null)
            {
                userProfile = new UserProfile
                {
                    Id = Guid.NewGuid(),
                    UserId = userId,
                    FullName = string.Empty,
                    Address = string.Empty,
                    AvatarUrl = string.Empty,
                    CCCDUrl = string.Empty
                };
                await _userProfileRepository.AddAsync(userProfile);
            }

            // Create upload path
            var uploadPath = Path.Combine("uploads", userId);
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // Generate unique filename
            var fileExtension = Path.GetExtension(request.File.FileName);
            var fileName = $"{request.DocumentType}_{DateTime.UtcNow:yyyyMMddHHmmss}{fileExtension}";
            var filePath = Path.Combine(uploadPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await request.File.CopyToAsync(stream);
            }

            // Update user profile based on document type
            var fileUrl = $"/uploads/{userId}/{fileName}";
            switch (request.DocumentType.ToUpper())
            {
                case "CCCD":
                    userProfile.CCCDUrl = fileUrl;
                    break;
                case "AVATAR":
                    userProfile.AvatarUrl = fileUrl;
                    break;
                default:
                    throw new Exception("Invalid document type. Only CCCD and AVATAR are supported.");
            }

            await _userProfileRepository.UpdateAsync(userProfile);
            return fileUrl;
        }

        public async Task<bool> VerifyUserAsync(string userId, string status)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Status = status;
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<UserProfileResponse?> AdminUpdateUserAsync(string userId, AdminUpdateUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return null;
            }

            var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (userProfile == null)
            {
                return null;
            }

            // Update user basic info
            if (!string.IsNullOrEmpty(request.FullName))
            {
                userProfile.FullName = request.FullName;
            }

            if (request.Dob.HasValue)
            {
                userProfile.Dob = request.Dob.Value;
            }

            if (!string.IsNullOrEmpty(request.Address))
            {
                userProfile.Address = request.Address;
            }

            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                user.PhoneNumber = request.PhoneNumber;
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                user.Status = request.Status;
            }

            // Update role if specified
            if (!string.IsNullOrEmpty(request.Role))
            {
                var currentRoles = await _userManager.GetRolesAsync(user);
                await _userManager.RemoveFromRolesAsync(user, currentRoles);
                await _userManager.AddToRoleAsync(user, request.Role);
            }

            // Save changes
            await _userManager.UpdateAsync(user);
            await _userProfileRepository.UpdateAsync(userProfile);

            return await GetUserProfileAsync(userId);
        }

        public async Task<bool> ChangeUserRoleAsync(string userId, string newRole)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Remove all current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Add new role
            var result = await _userManager.AddToRoleAsync(user, newRole);
            return result.Succeeded;
        }

        public async Task<bool> LockUserAsync(string userId, string? reason)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Status = "locked";
            var result = await _userManager.UpdateAsync(user);

            // TODO: Log reason to system log

            return result.Succeeded;
        }

        public async Task<bool> UnlockUserAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.Status = "active";
            var result = await _userManager.UpdateAsync(user);
            return result.Succeeded;
        }

        public async Task<bool> DeleteUserAsync(string userId, string? reason)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // TODO: Log reason to system log before deleting

            // First delete user profile
            var userProfile = await _userProfileRepository.GetByUserIdAsync(userId);
            if (userProfile != null)
            {
                await _userProfileRepository.DeleteAsync(userProfile.Id);
            }

            // Then delete user
            var result = await _userManager.DeleteAsync(user);

            return result.Succeeded;
        }

        public async Task<List<UserProfileResponse>> GetUsersByRoleAsync(string role)
        {
            // Get all users in the specified role
            var usersInRole = await _userManager.GetUsersInRoleAsync(role);

            var userProfileResponses = new List<UserProfileResponse>();

            foreach (var user in usersInRole)
            {
                var userProfile = await _userProfileRepository.GetByUserIdAsync(user.Id);

                userProfileResponses.Add(new UserProfileResponse
                {
                    UserId = user.Id,
                    Email = user.Email ?? string.Empty,
                    UserName = user.UserName ?? string.Empty,
                    FullName = userProfile?.FullName ?? string.Empty,
                    Dob = userProfile?.Dob,
                    Address = userProfile?.Address ?? string.Empty,
                    AvatarUrl = userProfile?.AvatarUrl ?? string.Empty,
                    CCCDUrl = userProfile?.CCCDUrl ?? string.Empty,
                    PhoneNumber = user.PhoneNumber ?? string.Empty,
                    Status = user.Status ?? "Active",
                    Role = role
                });
            }

            return userProfileResponses;
        }
    }
}
