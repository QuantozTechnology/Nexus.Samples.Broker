namespace Nexus.Samples.Broker.ViewModels
{
    public class BuyInfoModel
    {
        public string DCCode { get; set; }
        public bool AccountValid { get; set; }
        public PaymentMethod[] PaymentMethods { get; set; }
        public bool PaymentPending { get; set; }
        public bool CoolingDown { get; set; }
        public string Currency { get; set; }
        public LimitInfoModel Limits { get; set; }
    }

    public class LimitInfoModel {
        public string[] LimitReasons { get; set; }
        public decimal RemainingDailyLimit { get; set; }
        public decimal MinimumAmount { get; set; }
    }

    public class PaymentMethod
    {
        public string PaymentMethodName { get; set; }
        public string PaymentMethodCode { get; set; }
        public string PaymentTypeCode { get; set; }
        public string PaymentTypeDisplay { get; set; }
        public double MaxAmount { get; set; }
        public double MinAmount { get; set; }
    }
}
