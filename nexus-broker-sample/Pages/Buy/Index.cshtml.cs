using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Nexus.Samples.Broker.Pages.Buy
{
    public class BuySimulateModel : PageModel
    {
        [MaxLength(8)]
        public string AccountCode { get; set; }
        public string PaymentMethodCode { get; set; }
        public double Amount { get; set; }
        public string Currency { get; set; }
        public string Crypto { get; set; }
        public string CurrencyCode { get; set; }

        public void OnGet([FromRoute]string crypto, string id)
        {
            Crypto = crypto;
            CurrencyCode = "EUR";
            AccountCode = id;
        }
    }
}