using Microsoft.AspNetCore.Mvc;
using BancoSimulado.Data;
using BancoSimulado.Models;

namespace BancoSimulado.Controllers
{
    [ApiController]
    [Route("transferencias")]
    public class TransferenciaController : ControllerBase
    {
        [HttpPost]
        public IActionResult Transferir([FromBody] TransferenciaRequest request)
        {
            if (request.Moneda != "BOB")
                return BadRequest("Solo se permite moneda Bs");
            if (request.Monto <= 0)
                return BadRequest("El monto debe ser mayor a cero");
            if (request.CuentaOrigenId == request.CuentaDestinoId)
                return BadRequest("La cuenta origen y destino no pueden ser la misma");

            var cuentaOrigen = BancoData.Cuentas.FirstOrDefault(c => c.Id == request.CuentaOrigenId);
            var cuentaDestino = BancoData.Cuentas.FirstOrDefault(c => c.Id == request.CuentaDestinoId);

            if (cuentaOrigen == null || cuentaDestino == null)
                return NotFound("Cuenta no encontrada");

            if (cuentaOrigen.BloqueadaRetiro)
                return BadRequest("Cuenta origen bloqueada para retiro");

            if (cuentaDestino.BloqueadaDeposito)
                return BadRequest("Cuenta destino bloqueada para depósito");

            if (cuentaOrigen.Saldo < request.Monto)
                return BadRequest("Saldo insuficiente");
            // Débito
            cuentaOrigen.Saldo -= request.Monto;
            BancoData.Movimientos.Add(new Movimiento
            {
                CuentaId = cuentaOrigen.Id,
                Tipo = "Débito",
                Monto = request.Monto,
                Fecha = DateTime.UtcNow
            });

            // Abono
            cuentaDestino.Saldo += request.Monto;
            BancoData.Movimientos.Add(new Movimiento
            {
                CuentaId = cuentaDestino.Id,
                Tipo = "Abono",
                Monto = request.Monto,
                Fecha = DateTime.UtcNow
            });

            var clienteDestino = BancoData.Clientes
                .First(c => c.Id == cuentaDestino.ClienteId);

            return Ok(new
            {
                mensaje = "Transferencia exitosa",
                clienteDestino = clienteDestino.Nombre
            });
        }
    }

    public class TransferenciaRequest
    {
        public int CuentaOrigenId { get; set; }
        public int CuentaDestinoId { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; }
    }
}
