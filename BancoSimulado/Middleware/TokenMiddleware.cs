using BancoSimulado.Security;

namespace BancoSimulado.Middleware
{
    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Dejar pasar login y refresh
            if (context.Request.Path.StartsWithSegments("/auth"))
            {
                await _next(context);
                return;
            }

            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();

            if (authHeader == null || !authHeader.StartsWith("Bearer "))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token requerido");
                return;
            }

            var token = authHeader.Replace("Bearer ", "");

            if (!TokenStore.Tokens.ContainsKey(token))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token inválido");
                return;
            }

            if (TokenStore.Tokens[token] < DateTime.UtcNow)
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Token expirado");
                return;
            }

            await _next(context);
        }
    }
}
