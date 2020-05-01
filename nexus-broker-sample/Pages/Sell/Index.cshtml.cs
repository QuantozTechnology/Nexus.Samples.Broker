using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Nexus.Samples.Broker.Pages.Sell
{
    public class IndexModel : PageModel
    {
        public string AccountCode { get; set; }
        public string BTCstr { get; set; }  // use string because of decimal delimiter issue with non-english browsers
        public string Currency { get; set; }
        public string CryptoCode { get; set; }

        public void OnGet(string id = null)
        {
            AccountCode = id;
        }
    }
}