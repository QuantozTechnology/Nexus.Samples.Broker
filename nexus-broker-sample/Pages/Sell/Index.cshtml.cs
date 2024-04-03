using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Nexus.Samples.Broker.Pages.Sell
{
    public class IndexModel : PageModel
    {
        public string AccountCode { get; set; }
        public decimal CryptoAmount { get; set; }
        public string Currency { get; set; }
        public string CryptoCode { get; set; }

        public void OnGet(string id = null)
        {
            AccountCode = id;
        }
    }
}