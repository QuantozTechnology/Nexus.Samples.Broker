using System;

namespace Nexus.Samples.Sdk.Models.Response
{
    public class GetBrokerTransactionResponse
    {
        public DateTime Created { get; set; }
        public string TransactionCode { get; set; }
        public string TransactionType { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string PortfolioCode { get; set; }
        public string CustomerCode { get; set; }
        public string AccountCode { get; set; }
        public string CryptoCurrencyCode { get; set; }
        public string CurrencyCode { get; set; }
        public string PaymentMethodCode { get; set; }
        public string CryptoSendTxId { get; set; }
        public string CryptoReceiveTxId { get; set; }
        public DateTime? Notified { get; set; }
        public DateTime? Confirmed { get; set; }
        public DateTime? Finished { get; set; }
        public string Comment { get; set; }
        public decimal CryptoAmount { get; set; }
        public decimal CryptoSent { get; set; }
        public decimal CryptoTraded { get; set; }
        public decimal CryptoEstimatePrice { get; set; }
        public decimal CryptoTradePrice { get; set; }
        public decimal CryptoPrice { get; set; }
        public decimal TradeValue { get; set; }
        public decimal BankCommission { get; set; }
        public decimal PartnerCommission { get; set; }
        public decimal NetworkCommission { get; set; }
        public decimal? Payout { get; set; }
        public string PayComment { get; set; }
        public string BankTransferReference { get; set; }
    }
}
