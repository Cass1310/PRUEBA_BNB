namespace BancoSimulado.Models
{
    public class PagoQrOpenBankingRequest
    {
        public decimal amount { get; set; }
        public string currency { get; set; }
        public DateTime expirationDate { get; set; }
        public bool singleUse { get; set; }
        public string additionalData { get; set; }
        public int destinationAccount { get; set; }
    }
}
