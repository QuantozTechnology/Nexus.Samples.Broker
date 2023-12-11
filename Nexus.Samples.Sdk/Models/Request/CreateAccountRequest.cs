using System;
using System.Collections.Generic;
using System.Text;

namespace Nexus.Samples.Sdk.Models.Request
{
    public class CreateAccountRequest
    {
        public CreateAccountRequestAccountType AccountType { get; set; }
        public string CustomerCryptoAddress { get; set; }
        public string CryptoCode { get; set; }
        public string IP { get; set; }
    }

    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum CreateAccountRequestAccountType
    {
        BROKER,
        BROKERBUYONLY,
        CUSTODIAN,
        TOKEN,
        TOKENSHARED
    }
}
