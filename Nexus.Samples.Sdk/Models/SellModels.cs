using System;

namespace Nexus.Samples.Sdk.Models
{
    public class SellInfoPT
    {
        public bool AccountValid { get; set; }
        public bool IsBusiness { get; set; }
        public bool HighRisk { get; set; }
        public string TrustLevel { get; set; }
        public decimal MinBtcAmount { get; set; }
        public decimal MaxBtcAmount { get; set; }
        public int FirstBuyStatus { get; set; }
        public string Currency { get; set; }
        public bool NeedPhotoID { get; set; }
        public bool IsIdentifiedFullCompliant { get; set; }
        public bool SellServiceAvailable { get; set; }
        public string DCCode { get; set; } = "BTC";
    }

    public class AccountSellResponsePT
    {
        public string AccountCode { get; set; }

        public string Email { get; set; }

        public string BankAccountNumber { get; set; }

        public string BtcAddress { get; set; }

        public string TransactionCode { get; set; }

        public decimal BtcAmount { get; set; }

        public decimal AfterFee { get; set; }

        public DateTime TransactionTimestamp { get; set; }

        public int TransactionFixedMinutes { get; set; }

        public string Currency { get; set; }
    }

    public class AccountSellPT
    {
        public string AccountCode { get; set; }
        public decimal CryptoAmount { get; set; }
        public string Currency { get; set; }
        public string Ip { get; set; }
        public string CryptoCode { get; set; }
    }
}
