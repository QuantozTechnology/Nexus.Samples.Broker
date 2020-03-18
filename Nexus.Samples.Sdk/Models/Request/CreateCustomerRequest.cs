namespace Nexus.Samples.Sdk.Models.Request
{
    public class CreateCustomerRequest
    {
        public string CustomerCode { get; set; }
        public string PortfolioCode { get; set; }
        public string TrustLevel { get; set; }
        public string Status { get; set; }
        public string CurrencyCode { get; set; }
        public string Email { get; set; }
        public Account[] Accounts { get; set; }
    }

    public class Account
    {
        public string AccountType { get; set; }
        public string CustomerCryptoAddress { get; set; }
        public string CryptoCode { get; set; }
        public string Ip { get; set; }
    }
}
