using BancoSimulado.Security;
using BancoSimulado.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddHttpClient();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 🚪 API Gateway: bloquear todo excepto /auth y /gateway
app.UseMiddleware<GatewayAuthMiddleware>();

// 🔐 Token global (todo menos /auth)
app.UseMiddleware<TokenMiddleware>();

// 🎯 Controllers al final
app.MapControllers();

app.Run();
