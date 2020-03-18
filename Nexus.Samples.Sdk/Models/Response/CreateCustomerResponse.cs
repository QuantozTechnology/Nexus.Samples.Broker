using System;

namespace Nexus.Samples.Sdk.Models.Response
{

    public class CreateCustomerResponse
    {
        public string CustomerCode { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public string BankAccount { get; set; }
        public string Status { get; set; }
        public string Trustlevel { get; set; }
        public string PortFolioCode { get; set; }
        public string CurrencyCode { get; set; }
        public CreateCustomerResponseAccount[] Accounts { get; set; }
    }

    public class CreateCustomerResponseAccount
    {
        public string Guid { get; set; }
        public string AccountCode { get; set; }
        public string CustomerCode { get; set; }
        public DateTime Created { get; set; }
        public string DcReceiveAddress { get; set; }
        public string CustomerCryptoAddress { get; set; }
        public string DcCode { get; set; }
        public string AccountStatus { get; set; }
    }
}
