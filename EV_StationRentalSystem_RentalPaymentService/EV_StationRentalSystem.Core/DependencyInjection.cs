
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using EV_StationRentalSystem.Core.ServiceContracts;
using EV_StationRentalSystem.Core.Services;


namespace EV_StationRentalSystem.Core
{
    public static class DependencyInjection
    {
        // Extension method to add core services to the dependency injection container

        public static IServiceCollection AddCore(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IJwtService, JwtService>();

            services.AddScoped<IRentalOrderService, RentalOrderService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IAnalyticsService, AnalyticsService>();

            services.AddScoped<IRentalContractService, RentalContractService>();
            services.AddScoped<ICheckinService, CheckinService>();
            services.AddScoped<ICheckoutService, CheckoutService>();
            services.AddScoped<IPenaltyService, PenaltyService>();

            return services;
        }
    }
}
