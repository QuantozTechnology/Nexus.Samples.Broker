namespace Nexus.Samples.Broker.ViewModels
{
    public class CryptoSelectorViewModel
    {
        public string Crypto { get; set; }
        public string Page { get; set; }
        public string Currency { get; set; }
        public bool ShowPrices { get; set; } = true;
        public string Action { get; set; }

        public CryptoSelectorViewModel(string currency, string page, string crypto, bool showPrices = true, string action = "Index")
        {
            Crypto = crypto;
            Page = page;
            ShowPrices = showPrices;
            Currency = currency;
            Action = action;
        }
    }
}
