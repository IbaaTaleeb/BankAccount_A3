namespace BankAccountAssignment.API.Modules.JWT
{
    using Microsoft.AspNetCore.Http;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;

    public class JwtBlacklistMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtBlacklistMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var jwtService = context.RequestServices.GetRequiredService<JwtTokenService>();
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

            if (token != null && jwtService.IsTokenBlacklisted(token))
            {
                context.Response.StatusCode = 401; // Unauthorized
                await context.Response.WriteAsync("Invalid Operation");
                return;
            }

            await _next(context);
        }
    }

}
