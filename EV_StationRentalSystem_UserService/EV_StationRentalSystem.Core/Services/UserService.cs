

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

        public UserService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IMapper mapper, IJwtService jwtService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _jwtService = jwtService;
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
    }
}
