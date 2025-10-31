
using EV_StationRentalSystem.API.Middleware;
using EV_StationRentalSystem.Core;
using EV_StationRentalSystem.Core.HttpClients;
using EV_StationRentalSystem.Core.Mappers;
using EV_StationRentalSystem.Core.Policies;
using EV_StationRentalSystem.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace EV_StationRentalSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddCore();

            builder.Services.AddAutoMapper(typeof(FleetMappingProfile).Assembly);

            builder.Services.AddTransient<IUsersMicroservicePolicies, UsersMicroservicePolicies>();
            builder.Services.AddTransient<IPollyPolicies, PollyPolicies>();


            builder.Services
               .AddHttpClient<UserMicroClient>(client =>
               {
                   client.BaseAddress = new Uri($"https://{builder.Configuration["UserMicroName"]}:{builder.Configuration["UserMicroPort"]}");
               }).AddPolicyHandler(
                   builder.Services.BuildServiceProvider().GetRequiredService<IUsersMicroservicePolicies>().GetCombinedPolicy()
                );

            builder.Services.AddHttpClient<RentalPaymentMicroClient>(client =>
            {
                client.BaseAddress = new Uri($"https://{builder.Configuration["RentalPaymentMicroName"]}:{builder.Configuration["RentalPaymentMicroPort"]}");
            }).AddPolicyHandler(
                   builder.Services.BuildServiceProvider().GetRequiredService<IUsersMicroservicePolicies>().GetCombinedPolicy()
                );

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<FleetDbContext>();
                    context.Database.Migrate();
                    Console.WriteLine("Database migration completed successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database migration failed: {ex.Message}");
                    // Log error but continue running the application
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {

                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseCors();

            app.UseExceptionHandlingMiddleware();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
