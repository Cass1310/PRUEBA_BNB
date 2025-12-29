namespace BancoSimulado.Models
{
    public class PagoQrRequest
    {
        public string Glosa { get; set; }
        public decimal Monto { get; set; }
        public string Moneda { get; set; }
    }
}
