namespace Nexus.Samples.Sdk.Models.Response
{
    public class InitiateBrokerBuyResponse
    {
        public string TransactionCode { get; set; }
        public string Created { get; set; }
        public PaymentMethodResponse PaymentMethodInfo { get; set; }
        public string AccountCode { get; set; }
        public string CustomerCode { get; set; }
        public decimal? NetworkFee { get; set; }
        public decimal? ServiceFee { get; set; }
        public decimal? BankFee { get; set; }
        public string Comment { get; set; }
        public string Status { get; set; }
        public string BlockchainMessage { get; set; }
    }

    public class PaymentMethodResponse
    {
        public int TransactionFixedInMinutes { get; set; }
        public string PaymentMethodCode { get; set; }
        public string PaymentMethodName { get; set; }
    }
}
