
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
            services.AddDbContext<RentalPaymentDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("RentalPaymentDb")));

            // Register repositories
            services.AddScoped<IRentalOrderRepository, RentalOrderRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            services.AddScoped<IRentalContractRepository, RentalContractRepository>();
            services.AddScoped<ICheckinRepository, CheckinRepository>();
            services.AddScoped<ICheckoutRepository, CheckoutRepository>();
            services.AddScoped<IPenaltyRepository, PenaltyRepository>();

            return services;
        }
    }
}
