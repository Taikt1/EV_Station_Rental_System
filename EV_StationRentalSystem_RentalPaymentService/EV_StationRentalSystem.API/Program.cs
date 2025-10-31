
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

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddCore();

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddAutoMapper(typeof(RentalPaymentMappingProfile).Assembly);


            //Cors
            builder.Services.AddCors(options => {
                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins()
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            builder.Services.AddTransient<IUsersMicroservicePolicies, UsersMicroservicePolicies>();
            builder.Services.AddTransient<IPollyPolicies, PollyPolicies>();


            // Configure CORS to allow requests from specific origins
            builder.Services
                .AddHttpClient<UserMicroClient>(client =>
                {
                    client.BaseAddress = new Uri($"https://{builder.Configuration["UserMicroName"]}:{builder.Configuration["UserMicroPort"]}");
                })
                .AddPolicyHandler(
                   builder.Services.BuildServiceProvider().GetRequiredService<IUsersMicroservicePolicies>().GetCombinedPolicy()
                );

            builder.Services.AddHttpClient<FleetMicroClient>(client =>
            {
                client.BaseAddress = new Uri($"https://{builder.Configuration["FleetMicroName"]}:{builder.Configuration["FleetMicroPort"]}");
            }).AddPolicyHandler(
                   builder.Services.BuildServiceProvider().GetRequiredService<IUsersMicroservicePolicies>().GetCombinedPolicy()
                );

            var app = builder.Build();

            // Apply pending migrations at startup
            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<RentalPaymentDbContext>();
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
