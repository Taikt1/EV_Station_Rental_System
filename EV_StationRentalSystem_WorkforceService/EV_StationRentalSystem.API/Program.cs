
using EV_StationRentalSystem.API.Middleware;
using EV_StationRentalSystem.Core;
using EV_StationRentalSystem.Core.HttpClients;
using EV_StationRentalSystem.Core.Mappers;
using EV_StationRentalSystem.Core.Policies;
using EV_StationRentalSystem.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace EV_StationRentalSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddInfrastructure(builder.Configuration);
            builder.Services.AddCore();

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Tránh circular reference khi serialize JSON
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                    // Ignore null values để giảm kích thước response
                    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
                });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
                };
            });

            builder.Services.AddAutoMapper(typeof(WorkforceMappingProfile).Assembly);


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


            builder.Services.AddHttpClient<FleetMicroClient>(client =>
            {
                client.BaseAddress = new Uri($"https://{builder.Configuration["FleetMicroName"]}:{builder.Configuration["FleetMicroPort"]}");
            }).AddPolicyHandler(
                   builder.Services.BuildServiceProvider().GetRequiredService<IUsersMicroservicePolicies>().GetCombinedPolicy()
                );

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                try
                {
                    var context = scope.ServiceProvider.GetRequiredService<WorkforceDbContext>();
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

            // Add Gateway Auth Middleware to read headers from Gateway
            app.UseGatewayAuth();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
