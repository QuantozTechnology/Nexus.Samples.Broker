using System;

namespace Nexus.Samples.Sdk.Models.Response
{
    public class GetAccountResponse
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
