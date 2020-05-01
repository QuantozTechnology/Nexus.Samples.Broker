namespace Nexus.Samples.Sdk.Models.Request
{
    public class SimulateBuyBrokerRequest
    {
        public string AccountCode { get; set; }
        public string PaymentMethodCode { get; set; }
        public string Currency { get; set; }
        public decimal Amount { get; set; }
    }
}
