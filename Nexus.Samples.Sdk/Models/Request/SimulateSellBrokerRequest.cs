namespace Nexus.Samples.Sdk.Models.Request
{
    public class SimulateSellBrokerRequest
    {
        public string AccountCode { get; set; }
        public string PaymentMethodCode { get; set; }
        public decimal CryptoAmount { get; set; }
        public string Ip { get; set; }

    }
}
