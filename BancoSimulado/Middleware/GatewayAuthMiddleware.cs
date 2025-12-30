using BancoSimulado.Security;

namespace BancoSimulado.Security
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
            // 🔓 Login y refresh no pasan por gateway
            if (context.Request.Path.StartsWithSegments("/auth"))
            {
                await _next(context);
                return;
            }

            // Permitir llamadas internas del gateway
            if (context.Request.Headers.ContainsKey("X-Gateway-Forward"))
            {
                await _next(context);
                return;
            }

            // 🚪 Todo lo demás debe pasar por gateway
            if (!context.Request.Path.StartsWithSegments("/gateway"))
            {
                context.Response.StatusCode = 403;
                await context.Response.WriteAsync("Acceso solo permitido vía API Gateway");
                return;
            }
            await _next(context);
        }
    }
}
