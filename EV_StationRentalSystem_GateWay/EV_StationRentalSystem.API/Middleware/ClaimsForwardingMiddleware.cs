using System.Security.Claims;

namespace EV_StationRentalSystem.API.Middleware
{
    public class ClaimsForwardingMiddleware
    {
        private readonly RequestDelegate _next;

        public ClaimsForwardingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                // Forward user claims as headers to downstream services
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
             //?? context.User.FindFirst("nameid")?.Value;

                var userEmail = context.User.FindFirst(ClaimTypes.Email)?.Value
                             ?? context.User.FindFirst("email")?.Value;

                var userRole = context.User.FindFirst(ClaimTypes.Role)?.Value
                             ?? context.User.FindFirst("role")?.Value;

                var userName = context.User.FindFirst(ClaimTypes.Name)?.Value
                             ?? context.User.FindFirst("unique_name")?.Value
                             ?? context.User.FindFirst("name")?.Value;

                Console.WriteLine($"Forwarding Claims: UserId={userId}, UserEmail={userEmail}, UserRole={userRole}, UserName={userName}");

                if (!string.IsNullOrEmpty(userId))
                    context.Request.Headers["X-User-Id"] = userId;

                if (!string.IsNullOrEmpty(userEmail))
                    context.Request.Headers["X-User-Email"] = userEmail;

                if (!string.IsNullOrEmpty(userRole))
                    context.Request.Headers["X-User-Role"] = userRole;

                if (!string.IsNullOrEmpty(userName))
                    context.Request.Headers["X-User-Name"] = userName;

                // Debug: Log all claims
                //Console.WriteLine($"=== Claims for authenticated user ===");
                //foreach (var claim in context.User.Claims)
                //{
                //    Console.WriteLine($"Claim: {claim.Type} = {claim.Value}");
                //}
            }

            await _next(context);
        }
    }
}
