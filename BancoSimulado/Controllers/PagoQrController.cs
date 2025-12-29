using Microsoft.AspNetCore.Mvc;
using BancoSimulado.Models;

namespace BancoSimulado.Controllers
{
    [ApiController]
    [Route("pagos/qr")]
    public class PagoQrController : ControllerBase
    {
        [HttpPost]
        public IActionResult GenerarQr([FromBody] PagoQrRequest request)
        {
            // Validaciones básicas
            if (request.Moneda != "BOB")
                return BadRequest("Solo se permite moneda Bs");

            if (request.Monto <= 0)
                return BadRequest("El monto debe ser mayor a cero");

            if (string.IsNullOrWhiteSpace(request.Glosa))
                return BadRequest("La glosa es obligatoria");

            // Construcción del request Open Banking (PUENTE)
            var openBankingRequest = new PagoQrOpenBankingRequest
            {
                amount = request.Monto,
                currency = request.Moneda,
                expirationDate = DateTime.UtcNow.AddDays(1),
                singleUse = true,
                additionalData = request.Glosa,
                destinationAccount = 1
            };

            // 🚧 Simulación de consumo Open Banking
            // En un escenario real aquí se usa HttpClient
            var respuestaSimulada = new
            {
                qrId = Guid.NewGuid(),
                status = "GENERATED",
                expirationDate = openBankingRequest.expirationDate
            };

            return Ok(new
            {
                mensaje = "QR generado correctamente",
                qr = respuestaSimulada
            });
        }
    }
}
