using FluentValidation;
using EV_StationRentalSystem.Core.ServiceContracts;
using EV_StationRentalSystem.Core.Services;
using Microsoft.Extensions.DependencyInjection;

namespace EV_StationRentalSystem.Core
{
    public static class DependencyInjection
    {
        // Extension method to add core services to the dependency injection container

        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IWorkforceService, WorkforceService>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IUserReportService, UserReportService>();

            return services;
        }
    }
}
