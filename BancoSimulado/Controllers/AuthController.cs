using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using BancoSimulado.Security;

namespace BancoSimulado.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Validar email
            if (!Regex.IsMatch(request.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
                return BadRequest("Email inválido");

            // Validar password
            if (!Regex.IsMatch(request.Password, @"^[a-zA-Z0-9#&,_!]+$"))
                return BadRequest("Password inválido");

            // Usuarios hardcodeados (PDF)
            var usuariosValidos = new List<LoginRequest>
            {
                new LoginRequest { Email = "qwe@qwe.com", Password = "#qwe" },
                new LoginRequest { Email = "abc@abc.com", Password = "a&b!_" },
                new LoginRequest { Email = "zxc@zxc.com", Password = "1abc" }
            };

            var valido = usuariosValidos.Any(u =>
                u.Email == request.Email &&
                u.Password == request.Password
            );

            if (!valido)
                return Unauthorized("Credenciales incorrectas");

            var token = Guid.NewGuid().ToString();
            var refreshToken = Guid.NewGuid().ToString();

            var response = new LoginResponse
            {
                Token = token,
                RefreshToken = refreshToken,
                ExpiresInSeconds = 90
            };

            // Guardar token y expiración
            TokenStore.Tokens[token] = DateTime.UtcNow.AddSeconds(90);

            // Relacionar refresh con token
            TokenStore.RefreshTokens[refreshToken] = token;


            return Ok(response);
        }
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            if (!TokenStore.RefreshTokens.ContainsKey(request.RefreshToken))
                return Unauthorized("Refresh token inválido");

            // Invalidar token viejo
            var oldToken = TokenStore.RefreshTokens[request.RefreshToken];
            TokenStore.Tokens.Remove(oldToken);

            // Crear nuevo token
            var newToken = Guid.NewGuid().ToString();
            TokenStore.Tokens[newToken] = DateTime.UtcNow.AddSeconds(90);

            // Actualizar relación
            TokenStore.RefreshTokens[request.RefreshToken] = newToken;

            return Ok(new
            {
                token = newToken,
                expiresInSeconds = 90
            });
        }

    }

    public class LoginRequest
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public int ExpiresInSeconds { get; set; }
    }
    public class RefreshRequest
    {
        public string RefreshToken { get; set; }
    }

}
