using System;

namespace Nexus.Samples.Sdk.Models
{
    public class SellInfoRequestPT
    {
        public string AccountCode { get; set; }
        public string IP { get; set; }
    }

    public class SellInfoPT
    {
        public bool AccountValid { get; set; }
        public bool IsBusiness { get; set; }
        public string AccountType { get; set; }
        public double MinBtcAmount { get; set; }
        public double MaxBtcAmount { get; set; }
        public string Currency { get; set; }
        public bool SellServiceAvailable { get; set; }
        public string DCCode { get; set; } = "BTC";
    }

    public class AccountSellResponsePT
    {
        public string AccountCode { get; set; }

        public bool NeedAddressDetails { get; set; }

        public string Email { get; set; }

        public string BankAccountNumber { get; set; }

        public string BtcAddress { get; set; }

        public string TransactionCode { get; set; }

        public string Exchange { get; set; }

        public double BtcAmount { get; set; }

        public double AfterFee { get; set; }

        public DateTime TransactionTimestamp { get; set; }

        public int TransactionFixedMinutes { get; set; }

        public string Currency { get; set; }
    }

    public class AccountSellPT
    {
        public string AccountCode { get; set; }
        public string BTCstr { get; set; }
        public string Currency { get; set; }
        public string Ip { get; set; }
        public string CryptoCode { get; set; }
    }
}
