using Microsoft.AspNetCore.Mvc;
using BancoSimulado.Data;
namespace BancoSimulado.Controllers
{
    [ApiController]
    [Route("cuenta")]
    public class CuentaController : ControllerBase
    {
        [HttpGet("saldo")]
        public IActionResult GetSaldo()
        {
            return Ok(new
            {
                saldo = 1000,
                moneda = "BOB"
            });
        }
        [HttpGet("{clienteId}")]
        public IActionResult ObtenerCuenta(int clienteId)
        {
            var cuenta = BancoData.Cuentas.FirstOrDefault(c => c.ClienteId == clienteId);

            if (cuenta == null)
                return NotFound("Cuenta no encontrada");

            return Ok(cuenta);
        }
        [HttpPost("{id}/bloquear-deposito")]
        public IActionResult BloquearDeposito(int id)
        {
            var cuenta = BancoData.Cuentas.FirstOrDefault(c => c.Id == id);
            if (cuenta == null) return NotFound();

            cuenta.BloqueadaDeposito = true;
            return Ok("Cuenta bloqueada contra depósito");
        }

        [HttpPost("{id}/bloquear-retiro")]
        public IActionResult BloquearRetiro(int id)
        {
            var cuenta = BancoData.Cuentas.FirstOrDefault(c => c.Id == id);
            if (cuenta == null) return NotFound();

            cuenta.BloqueadaRetiro = true;
            return Ok("Cuenta bloqueada contra retiro");
        }
    }
}
