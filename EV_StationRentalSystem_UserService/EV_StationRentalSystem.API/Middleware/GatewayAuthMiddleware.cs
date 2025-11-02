using System.Security.Claims;

namespace EV_StationRentalSystem.API.Middleware
{
    public class GatewayAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public GatewayAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Check if request comes through Gateway (has Gateway headers)
            var userId = context.Request.Headers["X-User-Id"].FirstOrDefault();
            var userEmail = context.Request.Headers["X-User-Email"].FirstOrDefault();
            var userRole = context.Request.Headers["X-User-Role"].FirstOrDefault();
            var userName = context.Request.Headers["X-User-Name"].FirstOrDefault();

            // If Gateway headers exist, create user identity
            if (!string.IsNullOrEmpty(userId))
            {
                var claims = new List<Claim>
                {
                    new Claim("nameid", userId), // Use the same claim type as User Service expects

                    new Claim(ClaimTypes.NameIdentifier, userId)
                };
                Console.WriteLine($"Gateway Auth: Created user identity for {ClaimTypes.NameIdentifier}");


                if (!string.IsNullOrEmpty(userEmail))
                {
                    //claims.Add(new Claim("unique_name", userEmail));

                    claims.Add(new Claim(ClaimTypes.Email, userEmail));
                }

                if (!string.IsNullOrEmpty(userRole))
                {
                    claims.Add(new Claim("role", userRole));
                    claims.Add(new Claim(ClaimTypes.Role, userRole));
                }

                if (!string.IsNullOrEmpty(userName))
                {
                    claims.Add(new Claim(ClaimTypes.Name, userName));
                }

                var identity = new ClaimsIdentity(claims, "Gateway");
                context.User = new ClaimsPrincipal(identity);

                //Console.WriteLine($"Gateway Auth: Created user identity for {userId}");
            }

            await _next(context);
        }
    }

    public static class GatewayAuthMiddlewareExtensions
    {
        public static IApplicationBuilder UseGatewayAuth(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<GatewayAuthMiddleware>();
        }
    }
}
