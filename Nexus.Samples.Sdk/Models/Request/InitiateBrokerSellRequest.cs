namespace Nexus.Samples.Sdk.Models.Request
{
    public class InitiateBrokerSellRequest
    {
        public string IP {  get; set; }
        public string AccountCode { get; set; }
        public string PaymentMethodCode { get; set; }
        public decimal CryptoAmount { get; set; }
    }
}
