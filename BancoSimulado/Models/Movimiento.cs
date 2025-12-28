namespace BancoSimulado.Models
{
    public class Movimiento
    {
        public int CuentaId { get; set; }
        public string Tipo { get; set; } // Abono o Débito
        public decimal Monto { get; set; }
        public DateTime Fecha { get; set; }
    }
}
