namespace Nexus.Samples.Sdk.Models.Response
{
    public class SimulateSellBrokerResponse
    {
        public decimal ValueInFiatBeforeFees { get; set; }
        public decimal ValueInFiatAfterFees { get; set; }
        public decimal BankFee { get; set; }
        public decimal ServiceFee { get; set; }
        public decimal NetworkFee { get; set; }
    }
}
