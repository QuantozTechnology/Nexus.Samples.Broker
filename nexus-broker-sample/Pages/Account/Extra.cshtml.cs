using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Nexus.Samples.Sdk;
using Nexus.Samples.Sdk.Models.Request;

namespace Nexus.Samples.Broker.Pages.Account
{
    [BindProperties]
    public class ExtraModel : PageModel
    {
        private readonly NexusClient nexusClient;
        private readonly ILogger<ExtraModel> logger;

        [Required]
        public string AccountCode { get; set; }
        [Required]
        public string CustomerCryptoAddress { get; set; }
        public string CryptoCode { get; set; }

        public ExtraModel(NexusClient nexusClient, ILogger<ExtraModel> logger)
        {
            this.nexusClient = nexusClient;
            this.logger = logger;
        }

        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync()
        {

            var getAccountResponse = await nexusClient.GetAccount(AccountCode);

            if (!getAccountResponse.IsSuccess)
            {
                foreach (var error in getAccountResponse.Errors)
                {
                    logger.LogInformation(error);
                    switch (error)
                    {
                        case "AccountNotFound": ModelState.AddModelError(nameof(AccountCode), "Account not found."); break;
                        default:
                            break;
                    }
                }

                return Page();
            }

            var response = await nexusClient.CreateAccount(getAccountResponse.Values.CustomerCode, new CreateAccountRequest
            {
                AccountType = CreateAccountRequestAccountType.BROKER,
                CryptoCode = CryptoCode,
                CustomerCryptoAddress = CustomerCryptoAddress,
                IP = "::1"
            });

            if (response.Errors == null || response.Errors.Length == 0)
            {
                return RedirectToPage("/Account/Created");
            }
            else
            {
                foreach (var error in response.Errors)
                {
                    logger.LogInformation(error);
                    switch (error)
                    {
                        case "ERRORMESSAGE007": ModelState.AddModelError(nameof(CustomerCryptoAddress), "Invalid address."); break;
                        default:
                            break;
                    }
                }
            }

            return Page();
        }
    }
}