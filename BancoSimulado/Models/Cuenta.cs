namespace BancoSimulado.Models
{
    public class Cuenta
    {
        public int Id { get; set; }
        public int ClienteId { get; set; }
        public decimal Saldo { get; set; }
        public bool BloqueadaDeposito { get; set; }
        public bool BloqueadaRetiro { get; set; }
    }
}
