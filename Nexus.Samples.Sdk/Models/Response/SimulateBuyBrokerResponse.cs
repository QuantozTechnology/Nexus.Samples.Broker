namespace Nexus.Samples.Sdk.Models.Response
{
    public class SimulateBuyBrokerResponse
    {
        public decimal CryptoBuyPriceBeforeFee { get; set; }
        public decimal CryptoBuyPriceAfterServiceFee { get; set; }
        public decimal CryptoAmount { get; set; }
        public decimal CurrencyBankFee { get; set; }
        public decimal CurrencyServiceFee { get; set; }
        public float CurrencyNetworkFee { get; set; }
        public decimal TotalCurrency { get; set; }
        public decimal TotalCurrencyToPay { get; set; }
        public string CurrencyCode { get; set; }
        public string CryptoCode { get; set; }
    }
}
