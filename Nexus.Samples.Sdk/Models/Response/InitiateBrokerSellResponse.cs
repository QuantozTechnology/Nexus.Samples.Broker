namespace Nexus.Samples.Sdk.Models.Response
{
    public class InitiateBrokerSellResponse
    {
        public string AccountCode { get; set; }
        public string TransactionCode { get; set; }
        public string CustomerCode { get; set; }
        public string CryptoAddress { get; set; }
        public string CurrencyCode { get; set; }
        public decimal ValueInFiatBeforeFees { get; set; }
        public decimal ValueInFiatAfterFees { get; set; }
        public decimal NetworkFee { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal BankFee { get; set; }
    }
}
