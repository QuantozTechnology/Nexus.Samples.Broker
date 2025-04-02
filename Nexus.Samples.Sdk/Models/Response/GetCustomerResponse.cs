using System;

namespace Nexus.Samples.Sdk.Models.Response
{
    public class GetCustomerResponse
    {
        public string CustomerCode { get; set; }
        public string Email { get; set; }
        public DateTime Created { get; set; }
        public string BankAccount { get; set; }
        public string Status { get; set; }
        public string Trustlevel { get; set; }
        public string PortFolioCode { get; set; }
    }
}
