namespace Nexus.Samples.Sdk.Models.Request
{
    public class InitiateBrokerBuyRequest
    {
        public string PaymentMethodCode { get; set; }
        public string AccountCode { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string CallbackUrl { get; set; }
        public string BlockchainMessage { get; set; }
        public string IP { get; set; }
    }
}
