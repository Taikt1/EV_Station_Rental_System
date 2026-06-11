
using EV_StationRentalSystem.Core.RepositoryContracts;
using EV_StationRentalSystem.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EV_StationRentalSystem.Infrastructure
{
    public static class DependencyInjection
    {
        // Extension method to add infrastructure services to the dependency injection container

        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            // Add DbContext
            services.AddDbContext<FleetDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("FleetDB")));

            // Register repositories
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<ITypeVehicleRepository, TypeVehicleRepository>();
            services.AddScoped<IBranchDestinationRepository, BranchDestinationRepository>();
            services.AddScoped<IMaintenanceRecordRepository, MaintenanceRecordRepository>();
            services.AddScoped<IVehicleRelocationRepository, VehicleRelocationRepository>();
            services.AddScoped<IVehicleAvailabilityRepository, VehicleAvailabilityRepository>();

            return services;
        }
    }
}
