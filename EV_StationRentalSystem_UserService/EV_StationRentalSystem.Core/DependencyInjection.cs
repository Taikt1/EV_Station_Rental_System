using EV_StationRentalSystem.Core.ServiceContracts;
using EV_StationRentalSystem.Core.Services;
using EV_StationRentalSystem.Core.Validators;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;


namespace EV_StationRentalSystem.Core
{
    public static class DependencyInjection
    {
        // Extension method to add core services to the dependency injection container

        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IJwtService, JwtService>();

            // Register validators
            services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();

            return services;
        }
    }
}
