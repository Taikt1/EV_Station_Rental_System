
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EV_StationRentalSystem.API.Middleware;

namespace EV_StationRentalSystem.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add Ocelot configuration
            builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);

            // Add services to the container.
            builder.Services.AddControllers();

            // Add JWT Authentication
            var jwtSettings = builder.Configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["Key"] ?? "MyVeryLongSecretKeyForJWTTokenGeneration12345";
            var issuer = jwtSettings["Issuer"] ?? "EV_StationRentalSystem";
            var audience = jwtSettings["Audience"] ?? "EV_StationRentalSystem_Users";

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer("Bearer", options =>
                {
                    options.RequireHttpsMetadata = false; // Allow HTTP in development
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.Zero,
                        NameClaimType = "nameid", // Map name claim
                        RoleClaimType = "role"    // Map role claim
                    };
                });

            // Add Authorization
            builder.Services.AddAuthorization();

            // Add Ocelot
            builder.Services.AddOcelot();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
            builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
            builder.Logging.AddFilter("Microsoft.IdentityModel", LogLevel.Debug);

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            // Use CORS
            app.UseCors("AllowAll");

            // Use Authentication & Authorization
            app.UseAuthentication();
            app.UseAuthorization();

            // Use Claims Forwarding Middleware
            app.UseMiddleware<ClaimsForwardingMiddleware>();



            app.MapControllers();

            // Use Ocelot
            app.UseOcelot().Wait();

            app.Run();
        }
    }
}
