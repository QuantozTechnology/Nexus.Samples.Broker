using System;
using System.Collections.Generic;
using System.Text;

namespace Nexus.Samples.Sdk.Models.Request
{
    public class SimulateSellBrokerRequest
    {
        public string AccountCode { get; set; }
        public string PaymentMethodCode { get; set; }
        public int CryptoAmount { get; set; }
        public string Ip { get; set; }

    }
}
