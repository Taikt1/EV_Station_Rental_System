
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


            return services;
        }
    }
}
