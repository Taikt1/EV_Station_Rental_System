
using EV_StationRentalSystem.Core.ServiceContracts;
using EV_StationRentalSystem.Core.Services;
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
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<ITypeVehicleService, TypeVehicleService>();
            services.AddScoped<IBranchDestinationService, BranchDestinationService>();
            services.AddScoped<IMaintenanceRecordService, MaintenanceRecordService>();
            services.AddScoped<IVehicleRelocationService, VehicleRelocationService>();

            return services;
        }
    }
}
