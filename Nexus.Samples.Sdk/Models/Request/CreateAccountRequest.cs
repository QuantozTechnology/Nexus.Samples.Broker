using System.Text.Json.Serialization;

namespace Nexus.Samples.Sdk.Models.Request
{
    public class CreateAccountRequest
    {
        public CreateAccountRequestAccountType AccountType { get; set; }
        public string CustomerCryptoAddress { get; set; }
        public string CryptoCode { get; set; }
        public string IP { get; set; }
    }

    [JsonConverter(typeof(JsonStringEnumConverter))]
    [Newtonsoft.Json.JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
    public enum CreateAccountRequestAccountType
    {
        BROKER,
        BROKERBUYONLY,
        CUSTODIAN
    }
}
