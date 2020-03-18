using System.ComponentModel.DataAnnotations;

namespace Nexus.Samples.Sdk.Models
{
    public class BuyInfoPT //: PassthroughModel
    {
        public bool BuyServiceAvailable { get; set; }
        public bool AccountValid { get; set; }
        public bool IsBusiness { get; set; }
        public bool HighRisk { get; set; }
        public bool IsUnknownBank { get; set; }
        public string AccountType { get; set; }
        public string Currency { get; set; }
        public bool PaymentPending { get; set; }
        public bool CoolingDown { get; set; }
        public string CountryCode { get; set; }
        public int FirstBuyStatus { get; set; }
        public bool NeedFotoID { get; set; }
        public bool IsIdentifiedFullCompliant { get; set; }
        public CheckResponsePaymentMethods[] PaymentMethods { get; set; }
        public string DCCode { get; set; }
        public double MaxBuyAmount { get; set; }
        public double MinBuyAmount { get; set; }
        public string[] LimitReasons { get; set; }
    }

    public class CheckResponsePaymentMethods
    {
        public string PaymentMethodName { get; set; }
        public string PaymentMethodCode { get; set; }
        public string PaymentTypeCode { get; set; }
        public string PaymentTypeName { get; set; }
        public double MaxAmount { get; set; }
        public double MinAmount { get; set; }
    }

    public class BuyModel
    {
        [MaxLength(8)]
        public string AccountCode { get; set; }
        public string PaymentMethodCode { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Crypto { get; set; }
        public string CurrencyCode { get; set; }
    }
}
