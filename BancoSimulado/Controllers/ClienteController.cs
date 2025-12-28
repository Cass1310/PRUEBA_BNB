using Microsoft.AspNetCore.Mvc;
using BancoSimulado.Data;
using BancoSimulado.Models;

namespace BancoSimulado.Controllers
{
    [ApiController]
    [Route("clientes")]
    public class ClienteController : ControllerBase
    {
        [HttpPost]
        public IActionResult CrearCliente([FromBody] Cliente cliente)
        {
            cliente.Id = BancoData.Clientes.Count + 1;
            BancoData.Clientes.Add(cliente);

            var cuenta = new Cuenta
            {
                Id = BancoData.Cuentas.Count + 1,
                ClienteId = cliente.Id,
                Saldo = 1000,
                BloqueadaDeposito = false,
                BloqueadaRetiro = false
            };

            BancoData.Cuentas.Add(cuenta);

            return Ok(new
            {
                cliente,
                cuenta
            });
        }
    }
}
