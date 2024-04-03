namespace Nexus.Samples.Sdk.Models.Response
{
    public class GetBrokerLimitResponse
    {
        public LimitParameters LimitParameters { get; set; }
        public string[] LimitReasons { get; set; }
        public TrustlevelLimit Remaining { get; set; }
        public TrustlevelLimit Total { get; set; }
    }

    public class LimitParameters
    {
        public string CurrencyCode { get; set; }
        public string PaymentMethodCode { get; set; }
        public string CryptoCode { get; set; }
    }

    public class TrustlevelLimit
    {
        public decimal DailyLimit { get; set; }
        public decimal MonthlyLimit { get; set; }
        public decimal? YearlyLimit { get; set; }
        public decimal? LifetimeLimit { get; set; }
    }

}
